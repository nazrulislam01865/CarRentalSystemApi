using BLL.DTOs;
using DAL;
using DAL.EF.Models.Entities;
using DAL.EF.Models.Enums;
using DAL.Migrations;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class BookingService
    {
        readonly DataAccessFactory factory;
        private NotificationService Notifier => new NotificationService(factory);
        public BookingService(DataAccessFactory factory)
        {
            this.factory = factory;
        }
        //Notifiation
               
        private void NotifyStatusChange(Booking booking, BookingStatus oldStatus, BookingStatus newStatus)
        {
            if (oldStatus == newStatus) return;

            var type = newStatus switch
            {
                BookingStatus.Pending => NotificationType.BookingCreated,
                BookingStatus.Confirmed => NotificationType.BookingApproved,
                BookingStatus.Active => NotificationType.BookingStarted,
                BookingStatus.Completed => NotificationType.BookingCompleted,
                BookingStatus.Cancelled => NotificationType.BookingCancelled,
                _ => NotificationType.BookingCreated
            };

            // Customer        
            Notifier.NotifyUser(
                booking.CustomerId,
                type,
                "Booking Status Updated",
                $"Your booking #{booking.Id} status changed from {oldStatus} to {newStatus}.",
                booking.Id
            );

            // Admin/Staff        
            Notifier.NotifyAdminsAndStaff(
                type,
                "Booking Status Changed",
                $"Booking #{booking.Id} changed from {oldStatus} to {newStatus}.",
                booking.Id
            );
        }

        public List<BookingDTO> All()
        {
            var data = factory.BookingData().Get();
            return MapperConfig.GetMapper().Map<List<BookingDTO>>(data);
        }
        public List<BookingDTO> ByCustomer(int customerId)
        {
            var data = factory.BookingData().ByCustomer(customerId);
            return MapperConfig.GetMapper().Map<List<BookingDTO>>(data);
        }

        public BookingDTO Get(int id)
        {
            var data = factory.BookingData().Get(id);
            return MapperConfig.GetMapper().Map<BookingDTO>(data);
        }

        //public bool Create(BookingDTO dto)
        //{
        //    if (dto.CarId <= 0 || dto.CustomerId <= 0)
        //        return false;

        //    var start = dto.StartDate.Date;
        //    var end = dto.EndDate.Date;
        //    if (end <= start)
        //        return false;

        //    var car = factory.CarData().Get(dto.CarId);
        //    if (car == null) return false;
        //    if (car.Status == CarStatus.Maintenance) return false;


        //    dto.Status = BookingStatus.Pending;
        //    dto.ApprovedById = null;

        //    dto.TotalAmount = (decimal)(dto.EndDate - dto.StartDate).TotalDays * car.Deposit; ;
        //    var entity = MapperConfig.GetMapper().Map<Booking>(dto);
        //    return factory.BookingData().Create(entity);

        //}

        public bool Create(BookingDTO dto, out string message)
        {
            message = "";

            if (dto == null)
            {
                message = "Invalid booking data.";
                return false;
            }

            if (dto.CarId <= 0 || dto.CustomerId <= 0)
            {
                message = "Invalid CarId or CustomerId.";
                return false;
            }

            var start = dto.StartDate.Date;
            var end = dto.EndDate.Date;

            if (end <= start)
            {
                message = "EndDate must be after StartDate.";
                return false;
            }

            var car = factory.CarData().Get(dto.CarId);
            if (car == null)
            {
                message = "Car not found.";
                return false;
            }

            if (car.Status == CarStatus.Maintenance)
            {
                message = "Car is under maintenance.";
                return false;
            }

            // ✅ Block new bookings if this car already has Confirmed/Active booking overlap
            if (factory.BookingData().HasOverlappingConfirmedOrActiveBooking(dto.CarId, start, end))
            {
                message = "Car is already booked (confirmed/active) for this date range.";
                return false;
            }

            dto.Status = BookingStatus.Pending;
            dto.ApprovedById = null;
            dto.CreatedAt = DateTime.UtcNow;

            // Keep your existing pricing logic (you used Deposit)
            dto.TotalAmount = (decimal)(dto.EndDate - dto.StartDate).TotalDays * car.Deposit;

            var entity = MapperConfig.GetMapper().Map<Booking>(dto);
            var ok = factory.BookingData().Create(entity);

            if (ok)
            {
                Notifier.NotifyUser(dto.CustomerId,
                    NotificationType.BookingCreated,
                    "Booking Created",
                    $"Your booking #{entity.Id} has been created and is now Pending approval.",
                    entity.Id);

                Notifier.NotifyAdminsAndStaff(
                    NotificationType.BookingCreated,
                    "New Booking Request",
                    $"A new booking #{entity.Id} has been created and requires approval.",
                    entity.Id);
            }

            message = ok ? "Booking created (Pending)." : "Booking creation failed.";
            return ok;
        }
       
        public bool Complete(int bookingId, out string message)
        {
            message = "";

            var booking = factory.BookingData().Get(bookingId);
            if (booking == null)
            {
                message = "Booking not found.";
                return false;
            }

            if (booking.Status != BookingStatus.Active)
            {
                message = $"Booking cannot be completed because it is {booking.Status}.";
                return false;
            }

            var car = factory.CarData().Get(booking.CarId);
            if (car == null)
            {
                message = "Car not found.";
                return false;
            }
            var oldStatus = booking.Status;

            booking.Status = BookingStatus.Completed;
            var okBooking = factory.BookingData().Update(booking);
            if (!okBooking)
            {
                message = "Failed to complete booking.";
                return false;
            }

            car.Status = CarStatus.Available;
            var okCar = factory.CarData().Update(car);
            if (!okCar)
            {
                booking.Status = oldStatus;
                // rollback booking (best-effort)        
                booking.Status = BookingStatus.Active;
                factory.BookingData().Update(booking);

                message = "Failed to update car status to Available.";
                return false;
            }

            NotifyStatusChange(booking, oldStatus,booking.Status);

            message = "Booking completed. Car is now Available.";
            return true;
        }




        public bool Cancel(int id)
        {
            var booking = factory.BookingData().Get(id);
            if (booking == null) return false;
            if (booking.Status != BookingStatus.Pending) return false;
            if(booking.Status== BookingStatus.Confirmed) return false;
            booking.Status = BookingStatus.Cancelled;
            //return factory.BookingData().Update(booking);
            var oldStatus = booking.Status;
            booking.Status = BookingStatus.Cancelled;

            var ok = factory.BookingData().Update(booking);
            if (ok)
            {
                NotifyStatusChange(booking, oldStatus, booking.Status);
            }
            return ok;

        }
        public bool Delete(int id) {
            var booking =  factory.BookingData().Get(id);
            if (booking == null) return false;
            return factory.BookingData().Delete(id);
        }
        // Approve booking (Admin/Staff action)
        public bool Approve(int bookingId, int approverId, out string message)
        {
            message = "";

            var booking = factory.BookingData().Get(bookingId);
            if (booking == null)
            {
                message = "Booking not found.";
                return false;
            }

            if (booking.Status != BookingStatus.Pending)
            {
                message = $"Booking cannot be approved because it is {booking.Status}.";
                return false;
            }

            var approver = factory.UserData().Get(approverId);
            if (approver == null)
            {
                message = "Approver not found.";
                return false;
            }

            if (approver.Role == UserRole.Customer)
            {
                message = "Only Admin/Staff can approve bookings.";
                return false;
            }

            // Must be fully paid
            var paidSoFar = booking.Payments?
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount) ?? 0m;

            if (paidSoFar < booking.TotalAmount)
            {
                message = $"Payment not completed. Paid: {paidSoFar}, Due: {booking.TotalAmount}.";
                return false;
            }

            var oldStatus = booking.Status;
            booking.ApprovedById = approverId;
            booking.Status = BookingStatus.Confirmed;

            var ok = factory.BookingData().Update(booking);
            message = ok ? "Booking approved (Confirmed)." : "Booking approval failed.";
            if (ok)
            {
                NotifyStatusChange(booking, oldStatus, booking.Status);
            }
            return ok;
        }

        public bool Start(int bookingId, out string message)
        {
            message = "";

            var booking = factory.BookingData().Get(bookingId);
            if (booking == null)
            {
                message = "Booking not found.";
                return false;
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                message = $"Booking cannot be started because it is {booking.Status}.";
                return false;
            }

            var car = factory.CarData().Get(booking.CarId);
            if (car == null)
            {
                message = "Car not found.";
                return false;
            }

            if (car.Status != CarStatus.Available)
            {
                message = $"Car is not available. Current status: {car.Status}.";
                return false;
            }

            var oldStatus = booking.Status;
            booking.Status = BookingStatus.Active;
            var okBooking = factory.BookingData().Update(booking);
            if (!okBooking)
            {
                message = "Failed to start booking.";
                return false;
            }

            car.Status = CarStatus.Rented;
            var okCar = factory.CarData().Update(car);
            if (!okCar)
            {
                // rollback booking (best-effort)
                booking.Status = BookingStatus.Confirmed;
                factory.BookingData().Update(booking);

                message = "Failed to update car status to Rented.";
                return false;
            }
            NotifyStatusChange(booking, oldStatus, booking.Status);
            message = "Booking started. Car is now Rented.";
            return true;
        }

    }
}
