using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using HelpdeskSystem.API.Filters;
using HelpdeskSystem.API.Middleware;
using HelpdeskSystem.API.Services;
using HelpdeskSystem.Application.BackgroundJob;
using HelpdeskSystem.Application.Hubs;
using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Application.Mappings;
using HelpdeskSystem.Application.Services;
using HelpdeskSystem.Common.Settings;
using HelpdeskSystem.Infrastructure.Data;
using HelpdeskSystem.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Settings
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

// Database
builder.Services.AddDbContext<HelpdeskDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Hangfire Service
builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<ISlaMonitorJob, SlaMonitorJob>();

// SignalR
builder.Services.AddSignalR();


// Auth Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Ticket + Group Services
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IGroupService, GroupService>();

// User Service
builder.Services.AddScoped<IUserService, UserService>();

// Comment Service
builder.Services.AddScoped<ICommentService, CommentService>();

// Audit Logs Service
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// Notification Hub
builder.Services.AddScoped<INotificationService, NotificationService>();

// Dashboard Service
builder.Services.AddScoped<IDashboardService, DashboardService>();

// SLA Policy Service
builder.Services.AddScoped<ISlaPolicyService, SlaPolicyService>();

// Canned Response Service
builder.Services.AddScoped<ICannedResponseService, CannedResponseService>();

// JWT Authentication
JWTSettings jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero // No clock skew tolerance
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs/notifications"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

// Swagger with JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Helpdesk API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontendPolicy", policy => policy
            .WithOrigins("http://localhost:4200", "http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
        options.AddPolicy("AllowAngular", policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
        options.AddPolicy("AllowReact", policy => policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
        );
    }
);

// Fluent Validations
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Auto mapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AutoMappingProfile).Assembly));

// Controllers and Validation Filter Registration
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

// Middleware Pipeline
app.UseMiddleware<ExceptionMiddleware>();
app.UseHangfireDashboard();

// Schedule Job
RecurringJob.AddOrUpdate<SlaMonitorJob>("sla-Monitor", job => job.CheckBreachedTickets(), Cron.MinuteInterval(1));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseMiddleware<EncryptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapControllers();

app.Run();