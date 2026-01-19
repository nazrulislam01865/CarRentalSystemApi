using System;

namespace BLL.DTOs
{
    // Single month revenue result
    public class MonthlyRevenueDTO
    {
        public int Year { get; set; }

        // We keep MonthNumber internally (useful for sorting)
        public int MonthNumber { get; set; }

        // Optional for display (can still return it even if user sends month number/name)
        public string MonthName { get; set; } = string.Empty;

        public int TotalPaidTransactions { get; set; }
        public decimal TotalRevenue { get; set; }

        // The actual range used in calculation (UTC)
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
    }
}