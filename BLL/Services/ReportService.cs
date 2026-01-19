using BLL.DTOs;
using DAL;
using System.Globalization;

namespace BLL.Services
{
    public class ReportService
    {
        private readonly DataAccessFactory factory;

        public ReportService(DataAccessFactory factory)
        {
            this.factory = factory;
        }

        public MonthlyRevenueDTO RevenueMonthlySingle(int year, string monthInput)
        {
            var monthNumber = ParseMonth(monthInput);

            // Build UTC range: [first day of month, first day of next month)        
            var fromUtc = new DateTime(year, monthNumber, 1, 0, 0, 0, DateTimeKind.Utc);
            var toUtc = fromUtc.AddMonths(1);

            var paid = factory.PaymentData().GetPaidBetween(fromUtc, toUtc);

            return new MonthlyRevenueDTO
            {
                Year = year,
                MonthNumber = monthNumber,
                MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(monthNumber),
                TotalPaidTransactions = paid.Count,
                TotalRevenue = paid.Sum(x => x.Amount),
                FromUtc = fromUtc,
                ToUtc = toUtc
            };
        }

        // 1) Revenue daily (same logic)
        public List<RevenuePointDTO> RevenueDaily(DateTime fromUtc, DateTime toUtc)
        {
            var paid = factory.PaymentData().GetPaidBetween(fromUtc, toUtc);

            return paid
                .Where(p => p.PaidAt.HasValue)
                .GroupBy(p => p.PaidAt!.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => new RevenuePointDTO
                {
                    DateUtc = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc),
                    Amount = g.Sum(x => x.Amount)
                })
                .ToList();
        }

        // 2) Revenue monthly (UPDATED: includes MonthName)
        public List<RevenueMonthPointDTO> RevenueMonthly(DateTime fromUtc, DateTime toUtc)
        {
            var paid = factory.PaymentData().GetPaidBetween(fromUtc, toUtc);

            return paid
                .Where(p => p.PaidAt.HasValue)
                .GroupBy(p => new { p.PaidAt!.Value.Year, p.PaidAt!.Value.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new RevenueMonthPointDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    Amount = g.Sum(x => x.Amount)
                })
                .ToList();
        }

        // 3) Revenue range (UPDATED: hour-to-hour breakdown)
        public RevenueRangeHourlyDTO RevenueRangeHourly(DateTime fromUtc, DateTime toUtc)
        {
            var paid = factory.PaymentData().GetPaidBetween(fromUtc, toUtc);

            // Group paid amounts by hour bucket
            var byHour = paid
                .Where(p => p.PaidAt.HasValue)
                .GroupBy(p =>
                {
                    var t = p.PaidAt!.Value;
                    // Bucket to the hour (yyyy-mm-dd HH:00:00)
                    return DateTime.SpecifyKind(new DateTime(t.Year, t.Month, t.Day, t.Hour, 0, 0), DateTimeKind.Utc);
                })
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            // Build hourly timeline (fills missing hours with 0)
            var hours = new List<RevenueHourPointDTO>();

            var startHour = DateTime.SpecifyKind(new DateTime(fromUtc.Year, fromUtc.Month, fromUtc.Day, fromUtc.Hour, 0, 0), DateTimeKind.Utc);
            var endHour = DateTime.SpecifyKind(new DateTime(toUtc.Year, toUtc.Month, toUtc.Day, toUtc.Hour, 0, 0), DateTimeKind.Utc);

            for (var cur = startHour; cur < toUtc; cur = cur.AddHours(1))
            {
                byHour.TryGetValue(cur, out var amt);
                hours.Add(new RevenueHourPointDTO
                {
                    HourUtc = cur,
                    Amount = amt
                });
            }

            return new RevenueRangeHourlyDTO
            {
                FromUtc = fromUtc,
                ToUtc = toUtc,
                TotalPaidTransactions = paid.Count,
                TotalRevenue = paid.Sum(x => x.Amount),
                Hours = hours
            };
        }

        // ---------- helper ----------        
        private static int ParseMonth(string monthInput)
        {
            if (string.IsNullOrWhiteSpace(monthInput))
                throw new ArgumentException("Month is required.");

            monthInput = monthInput.Trim();

            // If month is numeric (1-12)        
            if (int.TryParse(monthInput, out var m))
            {
                if (m < 1 || m > 12)
                    throw new ArgumentException("Month number must be between 1 and 12.");
                return m;
            }

            // Month name parsing (January, Jan, FEB, etc.)        
            var dtfi = CultureInfo.InvariantCulture.DateTimeFormat;

            // full month names        
            for (int i = 1; i <= 12; i++)
            {
                var full = dtfi.GetMonthName(i);
                if (string.Equals(full, monthInput, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            // abbreviated month names (Jan, Feb, etc.)        
            for (int i = 1; i <= 12; i++)
            {
                var abbr = dtfi.GetAbbreviatedMonthName(i);
                if (string.Equals(abbr, monthInput, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            throw new ArgumentException("Invalid month. Use month number (1-12) or name (January/Jan).");
        }
    }
}