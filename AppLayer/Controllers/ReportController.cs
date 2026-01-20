using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace AppLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOrStaff")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService service;

        public ReportController(ReportService service)
        {
            this.service = service;
        }

        // 1) Revenue daily
        [HttpGet("RevenueDaily")]
        public IActionResult RevenueDaily(string fromUtc,string toUtc)
        {
            if (!TryParseUtc(fromUtc, out var from)) return BadRequest("Invalid fromUtc.");
            if (!TryParseUtc(toUtc, out var to)) return BadRequest("Invalid toUtc.");
            if (to <= from) return BadRequest("toUtc must be after fromUtc.");

            return Ok(service.RevenueDaily(from, to));
        }

        // 2) Revenue monthly 
        [HttpGet("RevenueMonthly")]
        public IActionResult RevenueMonthly([FromQuery] string month, [FromQuery] int? year = null)
        {
            // If year is not given default set to current  year        
            var y = year ?? DateTime.UtcNow.Year;

            try
            {
                var data = service.RevenueMonthlySingle(y, month);
                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 3) Revenue range 
        [HttpGet("RevenueRangeByHour")]
        public IActionResult RevenueRangeByHour(
            [FromQuery] string? dateUtc,
            [FromQuery] int fromHour,
            [FromQuery] int toHour)
        {
            if (fromHour < 0 || fromHour > 23) return BadRequest("fromHour must be 0-23.");
            if (toHour < 0 || toHour > 23) return BadRequest("toHour must be 0-23.");

            var baseDate = DateTime.UtcNow.Date;
            if (!string.IsNullOrWhiteSpace(dateUtc))
            {
                if (!TryParseUtcDateOnly(dateUtc, out baseDate))
                    return BadRequest("Invalid dateUtc. Use YYYY-MM-DD (example: 2026-01-20).");
            }

            var fromUtc = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, fromHour, 0, 0, DateTimeKind.Utc);
            var toUtcSameDay = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, toHour, 0, 0, DateTimeKind.Utc);

            // if cross midnight, add one day to toUtc
            var toUtc = (toHour <= fromHour) ? toUtcSameDay.AddDays(1) : toUtcSameDay;

            if (toUtc <= fromUtc) return BadRequest("Invalid hour range. toHour must create a later time than fromHour.");

            return Ok(service.RevenueRangeHourly(fromUtc, toUtc));
        }
        // UTC date-time parser
        private static bool TryParseUtc(string input, out DateTime utc)
        {
            if (DateTime.TryParse(input, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var dt))
            {
                utc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                return true;
            }

            utc = default;
            return false;
        }

        //  date-only parser (YYYY-MM-DD)
        private static bool TryParseUtcDateOnly(string input, out DateTime utcDate)
        {
            if (DateTime.TryParseExact(input.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dt))
            {
                utcDate = DateTime.SpecifyKind(dt.Date, DateTimeKind.Utc);
                return true;
            }

            utcDate = default;
            return false;
        }
    }
}
