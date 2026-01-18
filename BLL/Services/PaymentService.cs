using AutoMapper;
using BLL.DTOs;
using DAL;
using DAL.EF.Models.Entities;
using DAL.EF.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PaymentService
    {
        DataAccessFactory factory;
        public PaymentService(DataAccessFactory factory)
        {
            this.factory = factory;
        }

        public List<PaymentDTO> All()
        {
            var payments = factory.PaymentData().Get();
            var data = MapperConfig.GetMapper().Map<List<PaymentDTO>>(payments);
            return data;

        }

        public PaymentDTO Get(int id)
        {
            var payment = factory.PaymentData().Get(id);
            var data = MapperConfig.GetMapper().Map<PaymentDTO>(payment);
            return data;
        }

        public List<PaymentDTO> ByBooking(int bookingId)
        {
            var data = factory.PaymentData().GetByBooking(bookingId);
            return MapperConfig.GetMapper().Map<List<PaymentDTO>>(data);
        }

        //public bool Payment(PaymentDTO dto)
        //{
        //    if (dto.BookingId <= 0) return (false);
        //    if (dto.Amount <= 0) return (false);
        //    var booking = factory.BookingData().Get(dto.BookingId);
        //    if (booking == null) return (false);
        //    if (booking.Status == BookingStatus.Cancelled) return (false);

        //    dto.Status = PaymentStatus.Paid;
        //    var data = MapperConfig.GetMapper().Map<Payment>(dto);
        //    var result = factory.PaymentData().Create(data);
        //    return (result);

        //}

        public bool Payment(PaymentDTO dto, out string msg)
        {
            msg = string.Empty;

            if (dto == null)
            {
                msg = "Invalid payment data.";
                return false;
            }

            if (dto.BookingId <= 0)
            {
                msg = "Invalid BookingId.";
                return false;
            }

            if (dto.Amount <= 0)
            {
                msg = "Amount must be greater than 0.";
                return false;
            }

            // Helps when clients send an invalid int
            if (!Enum.IsDefined(typeof(PaymentMethod), dto.Method))
            {
                msg = "Invalid payment method.";
                return false;
            }

            var booking = factory.BookingData().Get(dto.BookingId);
            if (booking == null)
            {
                msg = "Booking not found.";
                return false;
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                msg = "Booking is cancelled. Payment is not allowed.";
                return false;
            }

            // Prevent duplicate/full over-payments
            var paidSoFar = booking.Payments?
                .Where(p => p.Status == PaymentStatus.Paid)
                .Sum(p => p.Amount) ?? 0m;

            var remaining = booking.TotalAmount - paidSoFar;
            if (remaining <= 0)
            {
                msg = "This booking is already fully paid.";
                return false;
            }

            // Simple rule: only accept the exact remaining amount
            if (dto.Amount != remaining)
            {
                msg = $"Invalid amount. Remaining due is {remaining}.";
                return false;
            }

            var data = MapperConfig.GetMapper().Map<Payment>(dto);

            data.Id = 0; // ensure EF treats it as new
            data.CreatedAt = DateTime.UtcNow;
            data.Status = PaymentStatus.Paid;
            data.PaidAt = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(data.TransactionRef))
            {
                data.TransactionRef = $"TXN-{Guid.NewGuid():N}";
            }

            var result = factory.PaymentData().Create(data);
            msg = result ? "Payment confirmed." : "Payment failed while saving to database.";
            return result;
        }



    }
}
