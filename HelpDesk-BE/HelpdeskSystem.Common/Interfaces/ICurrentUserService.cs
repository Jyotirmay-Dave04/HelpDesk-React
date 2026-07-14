namespace HelpdeskSystem.Application.Interfaces;

public interface ICurrentUserService
{
    int GetUserId();
    string GetUserName();
    string GetUserEmail();
    string GetUserRole();
}
