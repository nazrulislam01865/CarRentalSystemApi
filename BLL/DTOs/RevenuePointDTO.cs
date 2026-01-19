using System;

namespace BLL.DTOs
{
    // Revenue aggregated by day
    public class RevenuePointDTO
    {
        public DateTime DateUtc { get; set; }
        public decimal Amount { get; set; }
    }
}
