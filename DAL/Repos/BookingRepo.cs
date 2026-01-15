using DAL.EF;
using DAL.EF.Models.Entities;
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
                //.Include(x => x.Payments)
                .FirstOrDefault(x => x.Id == id);
        }

        public List<Booking> Get()
        {
            return db.Bookings
                //.Include(x => x.Payments)
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
    }
}
