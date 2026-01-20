using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        //Show All Bookings
        public IActionResult All()
        {
            var data = service.All();
            return Ok(data);
        }

        [HttpGet("Customer/{customerId}")]
        //Show Bookings by Customer
        public IActionResult ByCustomer(int customerId)
        {
            var data = service.ByCustomer(customerId);
            return Ok(data);
        }

        [HttpGet("{id}")]
        //Show Booking by Id
        public IActionResult Get(int id)
        {
            var data = service.Get(id);
            return Ok(data);
        }
        [HttpPost("Create")]
        //Create Booking
        public IActionResult Create(BookingDTO dto)
        {
            var data = service.Create(dto, out var msg);
            if (!data) return BadRequest("Somthing went wrong");
            return Ok(data);
        }
        [HttpDelete("Delete/{id}")]
        //Delete Booking
        public IActionResult Delete(int id)
        {
            var data =  service.Delete(id);
            if (!data) return BadRequest("Somthing went wrong");
            return Ok(data);
        }
        [HttpPost("Cancel/{BookingId}")]
        //Cancel Booking
        public IActionResult Cancel(int BookingId)
        {
            var data = service.Cancel(BookingId);
            if (!data) return BadRequest("Somthing went wrong");
            return Ok(data);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPost("Approve")]
        //Booking pending -> Approved by Admin/Staff
        public IActionResult Approve(ApproveBookingDTO dto)
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var approverId))
                return Unauthorized("Invalid authenticated user.");

            var ok = service.Approve(dto.BookingId, approverId, out var message);
            if (!ok) return BadRequest(message);

            return Ok(message);
        }

        [HttpPost("Start/{bookingId}")]
        //  Booking Approved -> Active + Car Unavailable
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
