using System;

namespace BLL.DTOs
{
    // Revenue aggregated by hour
    public class RevenueHourPointDTO
    {
        // Example: 2026-01-20T13:00:00Z
        public DateTime HourUtc { get; set; }
        public decimal Amount { get; set; }
    }
}