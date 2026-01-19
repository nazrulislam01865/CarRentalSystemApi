using BLL.DTOs;
using DAL;
using DAL.EF.Models.Entities;
using DAL.EF.Models.Enums;

namespace BLL.Services
{
    public class NotificationService
    {
        private readonly DataAccessFactory factory;

        public NotificationService(DataAccessFactory factory)
        {
            this.factory = factory;
        }

        public List<NotificationDTO> GetByUserId(int userId, bool unreadOnly = false, int take = 100)
        {
            var data = factory.NotificationData().ByUser(userId, unreadOnly, take);
            return MapperConfig.GetMapper().Map<List<NotificationDTO>>(data);
        }

        public bool MarkRead(int userId, int notificationId)
        {
            return factory.NotificationData().MarkRead(notificationId, userId);
        }

        public int MarkAllRead(int userId)
        {
            return factory.NotificationData().MarkAllRead(userId);
        }

        // Used internally from BookingService
        public void NotifyUser(int userId, NotificationType type, string title, string message, int? bookingId = null)
        {
            try
            {
                var n = new Notification
                {
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Message = message,
                    BookingId = bookingId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                factory.NotificationData().Create(n);
            }
            catch
            {
                
            }
        }

        public void NotifyAdminsAndStaff(NotificationType type, string title, string message, int? bookingId = null)
        {
            try
            {
                var users = factory.UserData().Get();
                var targets = users.Where(u => u.Role == UserRole.Admin || u.Role == UserRole.Staff).ToList();

                foreach (var u in targets)
                {
                    NotifyUser(u.Id, type, title, message, bookingId);
                }
            }
            catch
            {
               
            }
        }
    }
}