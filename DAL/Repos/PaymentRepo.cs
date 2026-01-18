using DAL.EF;
using DAL.EF.Models.Entities;
using DAL.Interfaces;
using DAL.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    public class PaymentRepo:IRepository<Payment>
    {
        UMSContext db;
        public PaymentRepo(UMSContext db)
        {
            this.db = db;
        }

        public bool Create(Payment p) {
            db.Payments.Add(p);
            return db.SaveChanges() > 0;
        }

        public Payment Get(int id) {
            return db.Payments.Find(id);
        }
        public List<Payment> Get() {
            return db.Payments.ToList();
        }
        public bool Update(Payment p) {
            var ex = Get(p.Id);
            db.Entry(ex).CurrentValues.SetValues(p);
            return db.SaveChanges() > 0;
        }
        public bool Delete(int id) {
            var ex = Get(id);
            db.Payments.Remove(ex);
            return db.SaveChanges() > 0;
        }

        public List<Payment> GetByBooking(int bookingId)
        {
            return db.Payments
                .Where(x=>x.BookingId == bookingId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
    }
}
