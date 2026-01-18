using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        readonly BookingService service;
        public BookingController(BookingService service)
        {
            this.service = service;
        }

        [HttpGet("All")]
        public IActionResult All()
        {
            var data = service.All();
            return Ok(data);
        }

        [HttpGet("Customer/{customerId}")]
        public IActionResult ByCustomer(int customerId)
        {
            var data = service.ByCustomer(customerId);
            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var data = service.Get(id);
            return Ok(data);
        }
        [HttpPost("Create")]
        public IActionResult Create(BookingDTO dto)
        {
            var data = service.Create(dto, out var msg);
            if (!data) return BadRequest("Somthing went wrong");
            return Ok(data);
        }
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var data =  service.Delete(id);
            if (!data) return BadRequest("Somthing went wrong");
            return Ok(data);
        }
        [HttpPost("Cancel/{BookingId}")]
        public IActionResult Cancel(int BookingId)
        {
            var data = service.Cancel(BookingId);
            if (!data) return BadRequest("Somthing went wrong");
            return Ok(data);
        }

        [HttpPost("Approve")]
        public IActionResult Approve(ApproveBookingDTO dto)
        {
            var data = service.Approve(dto.BookingId,dto.ApprovedById, out var msg);
            if (!data) return BadRequest(msg);
            return Ok(msg);
        }

        [HttpPost("Start/{bookingId}")]
        public IActionResult Start(int bookingId)
        {
            var ok = service.Start(bookingId, out var message);
            if (!ok) return BadRequest(message);
            return Ok(message);
        }

        //  Booking Active -> Completed + Car Available
        [HttpPost("Complete/{bookingId}")]
        public IActionResult Complete(int bookingId)
        {
            var ok = service.Complete(bookingId, out var message);
            if (!ok) return BadRequest(message);
            return Ok(message);
        }

    }
}
