namespace HelpdeskSystem.Application.Interfaces;

public interface INotificationHub
{
    public Task JoinTicketGroup(int ticketId);
    public Task LeaveTicketGroup(int ticketId);
}
