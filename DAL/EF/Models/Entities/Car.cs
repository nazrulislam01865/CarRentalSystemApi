using DAL.EF.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Models.Entities
{
    public class Car
    {
        public int Id { get; set; } 
        public string PlateNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Color { get; set; }
        public int Mileage { get; set; }


        // Pricing (moderate features: booking + payment)
        public decimal DailyRate { get; set; }
        public decimal Deposit { get; set; }

        // Availability status ("Rented" can be set when an active booking starts)
        public CarStatus Status { get; set; } = CarStatus.Available;

        // Navigation
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    }
}
