using DAL.EF;
using DAL.EF.Models.Entities;
using DAL.EF.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    public class BookingRepo
    {
        readonly UMSContext db;
        public BookingRepo(UMSContext db)
        {
            this.db = db;
        }

        public bool Create(Booking b)
        {
            db.Bookings.Add(b);
            return db.SaveChanges() > 0;
        }
        public Booking Get(int id)
        {
            return db.Bookings
                .Include(x => x.Payments)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<Booking> Get()
        {
            return db.Bookings
                .Include(x => x.Payments)
                .ToList();
        }
        public bool Update(Booking b)
        {
            var ex = db.Bookings.Find(b.Id);
            if (ex == null) return false;
            db.Entry(ex).CurrentValues.SetValues(b);
            return db.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var ex = db.Bookings.Find(id);
            if (ex == null) return false;
            db.Bookings.Remove(ex);
            return db.SaveChanges() > 0;
        }

        //=================================================\\
        public List<Booking> ByCustomer(int CId)
        {
            return db.Bookings
                .Include(x => x.Payments)
                .Where(x => x.CustomerId == CId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
        public List<Booking> GetBookedInRange(DateTime startDate, DateTime endDate)
        {
            // Overlapping any Confirmed/Active booking
            return db.Bookings
                .Include(x => x.Payments)
                .Where(b => (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Active)
                            && startDate < b.EndDate
                            && endDate > b.StartDate)
                .ToList();
        }
        //public decimal GetPaidAmount(int bookingId)
        //{
        //    return db.Payments
        //        .Where(p => p.BookingId == bookingId && p.Status == PaymentStatus.Paid)
        //        .Sum(p => (decimal?)p.Amount) ?? 0m;
        //}
        public bool HasOverlappingConfirmedOrActiveBooking(int carId, DateTime startDate, DateTime endDate, int? excludeBookingId = null)
        {
            var q = db.Bookings.Where(b =>
                b.CarId == carId &&
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Active) &&
                startDate < b.EndDate &&
                endDate > b.StartDate
            );

            if (excludeBookingId.HasValue)
                q = q.Where(b => b.Id != excludeBookingId.Value);

            return q.Any();
        }

    }
}
