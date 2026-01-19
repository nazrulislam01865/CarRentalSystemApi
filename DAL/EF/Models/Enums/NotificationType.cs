using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.EF.Models.Enums

{
    public enum NotificationType
    {
        BookingCreated = 0,
        PaymentConfirmed = 1,
        BookingApproved = 2,
        BookingStarted = 3,
        BookingCompleted = 4,
        BookingCancelled = 5
    }
    
}

