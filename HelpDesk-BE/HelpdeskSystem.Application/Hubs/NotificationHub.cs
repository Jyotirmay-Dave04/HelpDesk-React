using System.Security.Claims;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace HelpdeskSystem.Application.Hubs;

[Authorize]
public class NotificationHub: Hub
{
    public override async Task OnConnectedAsync()
    {
        string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(userId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

        string? role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        switch (role) {
            case nameof(UserRole.Admin): 
                await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
                break;
            
            case nameof(UserRole.Agent):
                await Groups.AddToGroupAsync(Context.ConnectionId, "agents");
                break;
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(userId is not null)
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");

        string? role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        switch (role) {
            case nameof(UserRole.Admin): 
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admins");
                break;
            
            case nameof(UserRole.Agent):
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "agents");
                break;
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinTicketGroup(int ticketId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
    }
    
    public async Task LeaveTicketGroup(int ticketId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket-{ticketId}");
    }
}
