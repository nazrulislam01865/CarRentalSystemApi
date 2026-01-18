using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthTestController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = "Basic")]
        [HttpGet("WhoAmI")]
        public IActionResult WhoAmI()
        {
            return Ok(new
            {
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = User.Identity?.Name,
                Role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
