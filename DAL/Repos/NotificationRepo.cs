using DAL.EF;
using DAL.EF.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repos
{
    public class NotificationRepo
    {
        private readonly UMSContext db;

        public NotificationRepo(UMSContext db)
        {
            this.db = db;
        }

        public bool Create(Notification n)
        {
            db.Notifications.Add(n);
            return db.SaveChanges() > 0;
        }

        public List<Notification> ByUser(int userId, bool unreadOnly, int take = 100)
        {
            var q = db.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId);

            if (unreadOnly)
            {
                q = q.Where(x => !x.IsRead);
            }

            return q.OrderByDescending(x => x.CreatedAt)
                    .Take(take)
                    .ToList();
        }

        public bool MarkRead(int notificationId, int userId)
        {
            var ex = db.Notifications.FirstOrDefault(x => x.Id == notificationId && x.UserId == userId);
            if (ex == null) return false;
            if (ex.IsRead) return true;

            ex.IsRead = true;
            return db.SaveChanges() > 0;
        }

        public int MarkAllRead(int userId)
        {
            var list = db.Notifications.Where(x => x.UserId == userId && !x.IsRead).ToList();
            if (!list.Any()) return 0;

            foreach (var n in list)
            {
                n.IsRead = true;
            }

            db.SaveChanges();
            return list.Count;
        }
    }
}