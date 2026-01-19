using System;
using System.Collections.Generic;

namespace BLL.DTOs
{

    public class RevenueRangeHourlyDTO
    {
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }

        public int TotalPaidTransactions { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<RevenueHourPointDTO> Hours { get; set; } = new();
    }
}