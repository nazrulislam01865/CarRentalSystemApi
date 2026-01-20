using System;

namespace BLL.DTOs
{
    
    public class MonthlyRevenueDTO
    {
        public int Year { get; set; }

        
        public int MonthNumber { get; set; }

       
        public string MonthName { get; set; }

        public int TotalPaidTransactions { get; set; }
        public decimal TotalRevenue { get; set; }

        
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
    }
}