//using BLL.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace AppLayer.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize(AuthenticationSchemes = "Basic")]
//    public class NotificationController : ControllerBase
//    {
//        private readonly NotificationService service;

//        public NotificationController(NotificationService service)
//        {
//            this.service = service;
//        }

//        private bool TryGetAuthenticatedUserId(out int userId)
//        {
//            userId = 0;
//            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            return int.TryParse(idStr, out userId);
//        }

//        // Customer/Admin/Staff -> own notifications
//        [HttpGet("Mine")]
//        public IActionResult Mine([FromQuery] bool unreadOnly = false, [FromQuery] int take = 100)
//        {
//            if (!TryGetAuthenticatedUserId(out var userId)) return Unauthorized("Invalid authenticated user.");

//            var data = service.GetByUserId(userId, unreadOnly, take);
//            return Ok(data);
//        }

//        // Admin/Staff -> search notifications by any user/customer id
//        [Authorize(Policy = "AdminOrStaff")]
//        [HttpGet("ByUser/{userId}")]
//        public IActionResult ByUser(int userId,  bool unreadOnly = false,  int take = 100)
//        {
//            var data = service.GetByUserId(userId, unreadOnly, take);
//            return Ok(data);
//        }

//        [HttpPost("Read/{id}")]
//        public IActionResult MarkRead(int id)
//        {
//            if (!TryGetAuthenticatedUserId(out var userId)) return Unauthorized("Invalid authenticated user.");

//            var ok = service.MarkRead(userId, id);
//            if (!ok) return BadRequest("Notification not found.");
//            return Ok("Marked as read.");
//        }

//        [HttpPost("ReadAll")]
//        public IActionResult MarkAllRead()
//        {
//            if (!TryGetAuthenticatedUserId(out var userId)) return Unauthorized("Invalid authenticated user.");

//            var count = service.MarkAllRead(userId);
//            return Ok(new { updated = count });
//        }
//    }
//}


using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace AppLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService service;

        public NotificationController(NotificationService service)
        {
            this.service = service;
        }

        // GET: /api/notification/mine?userId=5&unreadOnly=true&take=100
        [HttpGet("Mine")]
        public IActionResult Mine(int userId, bool unreadOnly = false, int take = 100)
        {
            if (userId <= 0) return BadRequest("userId is required.");

            var data = service.GetByUserId(userId, unreadOnly, take);
            return Ok(data);
        }

        // GET: /api/notification/byuser/5?unreadOnly=true&take=100
        [HttpGet("ByUser/{userId}")]
        public IActionResult ByUser(int userId, bool unreadOnly = false, int take = 100)
        {
            if (userId <= 0) return BadRequest("Invalid userId.");

            var data = service.GetByUserId(userId, unreadOnly, take);
            return Ok(data);
        }

        // POST: /api/notification/read/10?userId=5
        [HttpPost("Read/{id}")]
        public IActionResult MarkRead(int id, int userId)
        {
            if (userId <= 0) return BadRequest("userId is required.");

            var ok = service.MarkRead(userId, id);
            if (!ok) return BadRequest("Notification not found.");
            return Ok("Marked as read.");
        }

        // POST: /api/notification/readall?userId=5
        [HttpPost("ReadAll")]
        public IActionResult MarkAllRead(int userId)
        {
            if (userId <= 0) return BadRequest("userId is required.");

            var count = service.MarkAllRead(userId);
            return Ok(new { updated = count });
        }
    }
}