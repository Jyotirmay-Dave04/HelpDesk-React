using HelpdeskSystem.Application.Interfaces;
using HelpdeskSystem.Common.DTOs;
using HelpdeskSystem.Common.DTOs.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpdeskSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUser;

        public NotificationController(INotificationService notificationService, ICurrentUserService currentUserService)
        {
            _notificationService = notificationService;
            _currentUser = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] NotificationRequestFilterDto filterDto)
        {
            int userId = _currentUser.GetUserId();
            ApiResponse<PagedResponse<NotificationResponseDto>> result = await _notificationService.GetUserNotificationsAsync(userId, filterDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetMyNotificationsWithPost([FromBody] NotificationRequestFilterDto filterDto)
        {
            int userId = _currentUser.GetUserId();
            ApiResponse<PagedResponse<NotificationResponseDto>> result = await _notificationService.GetUserNotificationsAsync(userId, filterDto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("unreadCount")]
        public async Task<IActionResult> GetUnreadCount()
        {
            int userId = _currentUser.GetUserId();
            ApiResponse<int> result = await _notificationService.GetUnreadCountAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            int userId = _currentUser.GetUserId();
            ApiResponse<bool> result = await _notificationService.MarkAsReadAsync(id, userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("readAll")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            int userId = _currentUser.GetUserId();
            ApiResponse<bool> result = await _notificationService.MarkAllAsReadAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
