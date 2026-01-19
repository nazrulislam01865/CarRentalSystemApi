namespace BLL.DTOs
{
  
    public class RevenueMonthPointDTO
    {
        public int Year { get; set; }

        
        public int Month { get; set; }
       
        public string MonthName { get; set; }

        public decimal Amount { get; set; }
    }
}