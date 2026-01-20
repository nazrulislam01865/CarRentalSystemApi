using DAL.EF.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF
{
    public class UMSContext: DbContext
    {
        public UMSContext(DbContextOptions<UMSContext> opt) : base(opt)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.Role).HasConversion<int>();
                e.Property(x => x.Name).HasMaxLength(100);
                e.Property(x => x.Email).HasMaxLength(150);
                e.Property(x => x.Phone).HasMaxLength(30);
                e.Property(x => x.Password).HasMaxLength(200);
            });

            modelBuilder.Entity<Car>(e =>
            {
                e.HasIndex(x => x.PlateNumber).IsUnique();
                e.Property(x => x.Status).HasConversion<int>();
                e.Property(x => x.PlateNumber).HasMaxLength(30);
                e.Property(x => x.Brand).HasMaxLength(50);
                e.Property(x => x.Model).HasMaxLength(50);
                e.Property(x => x.Color).HasMaxLength(30);
                e.Property(x => x.DailyRate).HasPrecision(18, 2);
                e.Property(x => x.Deposit).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Booking>(e =>
            {
                e.Property(x => x.Status).HasConversion<int>();
                e.Property(x => x.TotalAmount).HasPrecision(18, 2);

                // Prevent cascade delete that can wipe bookings when a car/user is removed.
                e.HasOne(b => b.Car)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CarId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(b => b.Customer)
                    .WithMany(u => u.BookingsAsCustomer)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(b => b.ApprovedBy)
                    .WithMany(u => u.BookingsApproved)
                    .HasForeignKey(b => b.ApprovedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Helpful index for availability checks
                e.HasIndex(b => new { b.CarId, b.StartDate, b.EndDate });
            });

            modelBuilder.Entity<Notification>(e =>
            {
                e.Property(x => x.Type).HasConversion<int>();
                e.Property(x => x.Title).HasMaxLength(150);
                e.Property(x => x.Message).HasMaxLength(500);

                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Booking)
                    .WithMany()
                    .HasForeignKey(x => x.BookingId)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
            });
        }

    }
}
