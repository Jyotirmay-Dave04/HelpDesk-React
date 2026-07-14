using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ICurrentUserService _currentUser;

        public TicketController(ITicketService ticketService, ICurrentUserService currentUserService)
        {
            _ticketService = ticketService;
            _currentUser = currentUserService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll([FromQuery] TicketFilterDto filter)
        {
            ApiResponse<PagedResponse<TicketListItemDto>> result = await _ticketService.GetAllAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            ApiResponse<TicketResponseDto> result = await _ticketService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
        {
            ApiResponse<TicketResponseDto> result = await _ticketService.CreateAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update/{id:int}")]
        [Authorize(Roles = $"{nameof(UserRole.Requester)},{nameof(UserRole.Admin)}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
        {
            ApiResponse<TicketResponseDto> result = await _ticketService.UpdateAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Delete(int id)
        {
            ApiResponse<bool> result = await _ticketService.DeleteAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/assign")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> Assign(int id, [FromBody] AssignTicketDto dto)
        {
            ApiResponse<TicketResponseDto> result = await _ticketService.AssignAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            ApiResponse<TicketResponseDto> result = await _ticketService.UpdateStatusAsync(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("export")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> ExportCsv([FromQuery] TicketFilterDto filter)
        {
            var csvBytes = await _ticketService.ExportCsvAsync(filter);
            var fileName = $"tickets_{DateTime.UtcNow:yyyyMMdd_HHmm}.csv";
            return File(csvBytes, "text/csv", fileName);
        }

        [HttpPost("getAllWithPost")]
        public async Task<IActionResult> GetAllWithPost([FromBody] TicketFilterDto filter)
        {
            ApiResponse<PagedResponse<TicketListItemDto>> result = await _ticketService.GetAllAsync(filter);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
