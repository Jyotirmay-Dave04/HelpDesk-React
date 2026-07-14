namespace HelpdeskSystem.Common.DTOs.Comments;

public class CreateCommentDto
{
    public int TicketId { get; set; }
    public string Body { get; set; } = string.Empty;
    public bool IsInternal { get; set; } = false;
}
