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
        //Show All Users
        public IActionResult All()
        {
            var data = service.All();
            return Ok(data);
        }
        [HttpGet("{id}")]
        //Show User by Id
        public IActionResult Get(int id)
        {
            var data = service.Get(id);
            return Ok(data);
        }
        [HttpPost("create")]
        //Create User
        public IActionResult Create(UserDTO d)
        {

            var rs = service.Create(d);
            return Ok(rs);
        }

        [HttpPut("update")]
        //Update User
        public IActionResult Update( UserDTO user)
        {

            var updated = service.Update(user);

            if (!updated)
                return NotFound("User not found");

            return Ok("User updated successfully");
        }
        [HttpGet("delete/{id}")]
        //Delete User
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
