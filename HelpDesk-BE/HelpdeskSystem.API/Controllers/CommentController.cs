using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto createCommentDto)
        {
            ApiResponse<CommentResponseDto> result = await _commentService.CreateAsync(createCommentDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("ticket/{id:int}")]
        public async Task<IActionResult> GetCommentsByTicketId(int id)
        {
            ApiResponse<List<CommentResponseDto>> result = await _commentService.GetByTicketIdAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
