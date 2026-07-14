using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.Enums;
using HelpdeskSystem.Domain.Entities;
using HelpdeskSystem.Infrastructure.UnitOfWork;

namespace HelpdeskSystem.Application.BackgroundJob;

public class SlaMonitorJob : ISlaMonitorJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public SlaMonitorJob(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task CheckBreachedTickets()
    {
        DateTime now = DateTime.UtcNow;

        IEnumerable<Ticket> breachedTickets = await _unitOfWork.Tickets.FindAsync(t =>
            !t.SlaBreached &&
            !t.IsDeleted &&
            t.SlaPausedAt == null &&
            t.Status != TicketStatus.Rejected &&
            t.Status != TicketStatus.Resolved &&
            t.Status != TicketStatus.Closed &&
            t.Status != TicketStatus.Cancelled &&
            t.Status != TicketStatus.CannotResolve &&
            t.SlaDeadline < now
        );

        foreach(Ticket ticket in breachedTickets)
        {
            ticket.SlaBreached = true;
            AuditLog newLog = new AuditLog {
                TicketId = ticket.Id,
                Action = AuditAction.SlaBreached
            };
            await _unitOfWork.AuditLogs.AddAsync(newLog);

            if(ticket.AssignedTo.HasValue)
                await _notificationService.NotifyUserAsync(ticket.AssignedTo.Value, ticket.Id, NotificationType.SlaBreached, $"SLA breached on ticket #{ticket.Id}.");
            
            await _notificationService.NotifyRoleAsync(nameof(UserRole.Admin), ticket.Id, NotificationType.SlaBreached, $"SLA breached on ticket #{ticket.Id}.");
        }

        await _unitOfWork.SaveChangesAsync();
    }
}