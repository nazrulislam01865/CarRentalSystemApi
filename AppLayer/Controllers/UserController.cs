using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UserService service;
        public UserController(UserService service)
        {
            this.service = service;
        }
        [HttpGet("all")]
        public IActionResult All()
        {
            var data = service.All();
            return Ok(data);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var data = service.Get(id);
            return Ok(data);
        }
        [HttpPost("create")]
        public IActionResult Create(UserDTO d)
        {

            var rs = service.Create(d);
            return Ok(rs);
        }
        [HttpPut("update")]
        public IActionResult Update(UserDTO d)
        {
            if (d == null || d.Id == 0)
                return BadRequest("Invalid user data.");

            var result = service.Update(d); 

            if (result)
                return Ok(new { message = "User updated successfully." });
            else
                return NotFound(new { message = "User not found." });
        }
        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if(id == 0 || id== null)
            {
                return BadRequest("Invalid Id");
            }
            var data = service.Delete(id);

            if (data)
            {
                return Ok("User removed");
            }
            else
                return NotFound();
        }


    }
}
