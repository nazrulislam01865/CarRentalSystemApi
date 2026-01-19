using DAL.EF.Models.Enums;

namespace BLL.DTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public NotificationType Type { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? BookingId { get; set; }
    }
}