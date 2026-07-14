namespace HelpdeskSystem.Application.BackgroundJob;

public interface ISlaMonitorJob
{
    Task CheckBreachedTickets();
}