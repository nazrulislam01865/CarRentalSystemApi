using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        CarService carService;
        public CarController(CarService carService)
        {
            this.carService = carService;
        }
        [HttpGet("All")]
        public IActionResult All()
        {
            var data = carService.All();
            return Ok(data);

        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var data = carService.Get(id);
            return Ok(data);
        }
        [HttpPost("Create")]
        public IActionResult Create(CarDTO c)
        {
            var data = carService.Create(c);
            return Ok(data);
        }
        [HttpPut("Update")]
        public IActionResult Update(CarDTO c)
        {
            var data = carService.Update(c);
            return Ok(data);
        }
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var data = carService.Delete(id);
            return Ok(data);
        }
    }
}
