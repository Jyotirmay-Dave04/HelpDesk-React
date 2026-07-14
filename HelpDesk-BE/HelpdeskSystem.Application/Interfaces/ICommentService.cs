using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Comments;

namespace HelpdeskSystem.Application.Interfaces;

public interface ICommentService
{
    Task<ApiResponse<CommentResponseDto>> CreateAsync(CreateCommentDto createCommentDto);
    Task<ApiResponse<List<CommentResponseDto>>> GetByTicketIdAsync(int ticketId);
}
