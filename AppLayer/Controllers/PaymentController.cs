using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        readonly PaymentService service;
        public PaymentController(PaymentService service)
        {
            this.service = service;
        }
        [HttpGet("All")]
        //Show All Payments
        //public IActionResult All()
        //{
        //    var data = service.All();
        //    return Ok(data);
        //}
        [HttpGet("{id}")]
        //Show Payment by Id
        public IActionResult Get(int id)
        {
            var data = service.Get(id);
            return Ok(data);
        }
        [HttpGet("Booking/{bookingId}")]
        //Show Payment by Booking Id
        public IActionResult ByBooking(int bookingId)
        {
            var data = service.ByBooking(bookingId);
            return Ok(data);
        }

        [HttpPost("Pay")]
        // Endpoint to process a payment
        public IActionResult Pay(PaymentDTO dto)
        {
            var data = service.Payment(dto, out var msg);
            if (!data)
            {
                return BadRequest(msg);
            }
            return Ok(data);
        }
    }
}
