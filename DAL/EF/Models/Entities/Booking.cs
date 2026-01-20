using DAL.EF.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public virtual Car Car { get; set; }

        public int CustomerId { get; set; }
        public virtual User Customer { get; set; }
        public int? ApprovedById { get; set; }
        public virtual User ApprovedBy { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
