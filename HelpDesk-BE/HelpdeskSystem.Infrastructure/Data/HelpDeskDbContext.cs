using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Common;
using HelpdeskSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HelpdeskSystem.Infrastructure.Data;

public class HelpdeskDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    public HelpdeskDbContext(DbContextOptions<HelpdeskDbContext> options, ICurrentUserService currentUserService) : base(options) 
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SubCategory> SubCategories => Set<SubCategory>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>(); 
    public DbSet<SlaPolicy> SlaPolicies => Set<SlaPolicy>();
    public DbSet<CannedResponse> CannedResponses => Set<CannedResponse>();

    // Auto UTC timestamp on SaveChanges
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int currentUserId = _currentUserService.GetUserId();

        foreach (EntityEntry<BaseEntity> entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = currentUserId;
            }
            
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = DateTime.UtcNow;
                entry.Entity.ModifiedBy = currentUserId;
            }

            if (entry.State == EntityState.Deleted)
            {
                // Convert hard delete to soft delete automatically
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = currentUserId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .HasPostgresEnum<UserRole>()
            .HasPostgresEnum<TicketStatus>()
            .HasPostgresEnum<Priority>()
            .HasPostgresEnum<AuditAction>()
            .HasPostgresEnum<NotificationType>();

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Role).HasConversion<string>().IsRequired();
        });

        // Group
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
        });

        // Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);

            entity.HasOne(c => c.Group)
                  .WithMany(g => g.Categories)
                  .HasForeignKey(c => c.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // SubCategory
        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.Name).IsRequired().HasMaxLength(100);
            
            entity.HasOne(sc => sc.Category)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(sc => sc.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.ServiceDetails).IsRequired();
            entity.Property(t => t.Status).HasConversion<string>().IsRequired();
            entity.Property(t => t.Priority).HasConversion<string>().IsRequired();
            entity.Property(t => t.SlaBreached).HasDefaultValue(false);

            entity.HasOne(t => t.RequestedByUser)
                  .WithMany(u => u.RequestedTickets)
                  .HasForeignKey(t => t.RequestedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.AssignedToUser)
                  .WithMany(u => u.AssignedTickets)
                  .HasForeignKey(t => t.AssignedTo)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Group)
                  .WithMany(g => g.Tickets)
                  .HasForeignKey(t => t.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Category)
                  .WithMany(c => c.Tickets)
                  .HasForeignKey(t => t.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.SubCategory)
                  .WithMany(sc => sc.Tickets)
                  .HasForeignKey(t => t.SubCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Body).IsRequired();

            entity.HasOne(c => c.Ticket)
                  .WithMany(t => t.Comments)
                  .HasForeignKey(c => c.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Author)
                  .WithMany(u => u.Comments)
                  .HasForeignKey(c => c.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Action).HasConversion<string>().IsRequired();
            entity.Property(a => a.FieldName).HasMaxLength(100);
            
            entity.HasOne(a => a.Ticket)
                  .WithMany(t => t.AuditLogs)
                  .HasForeignKey(a => a.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(a => a.ChangedByUser)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(a => a.ChangedBy)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Type).HasConversion<string>().IsRequired();
            entity.Property(n => n.Message).HasMaxLength(500);
            entity.Property(n => n.IsRead).HasDefaultValue(false);
            
            entity.HasOne(n => n.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(n => n.Ticket)
                  .WithMany(t => t.Notifications)
                  .HasForeignKey(n => n.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // SLA Policy
        modelBuilder.Entity<SlaPolicy>(entity => 
        {
            entity.Property(s => s.Priority).HasConversion<string>().IsRequired();
            entity.HasIndex(s => s.Priority).IsUnique();
        });

        // Seed Data
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@helpdesk.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                CreatedBy = 0,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                Name = "Agent One",
                Email = "agent@helpdesk.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent@123"),
                Role = UserRole.Agent,
                CreatedBy = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Group>().HasData(
            new Group { Id = 1, Name = "IT Support",    CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Group { Id = 2, Name = "HR",            CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<Category>().HasData(
            // IT Support
            new Category { Id = 1, Name = "Hardware",  GroupId = 1, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 2, Name = "Software",  GroupId = 1, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 3, Name = "Network",   GroupId = 1, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // HR
            new Category { Id = 4, Name = "Payroll",   GroupId = 2, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = 5, Name = "Onboarding",GroupId = 2, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
        
        modelBuilder.Entity<SubCategory>().HasData(
            // Hardware
            new SubCategory { Id = 1, Name = "Laptop",   CategoryId = 1, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new SubCategory { Id = 2, Name = "Printer",  CategoryId = 1, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Software
            new SubCategory { Id = 3, Name = "OS Issue", CategoryId = 2, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new SubCategory { Id = 4, Name = "App Crash",CategoryId = 2, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Network
            new SubCategory { Id = 5, Name = "VPN",      CategoryId = 3, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new SubCategory { Id = 6, Name = "WiFi",     CategoryId = 3, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            // Payroll
            new SubCategory { Id = 7, Name = "Salary",   CategoryId = 4, CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<SlaPolicy>().HasData(
            new SlaPolicy { Id = 1, Priority = Priority.Low,    HoursToResolve = 24,    CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new SlaPolicy { Id = 2, Priority = Priority.Medium, HoursToResolve = 8,     CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new SlaPolicy { Id = 3, Priority = Priority.High,   HoursToResolve = 4,     CreatedBy = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
