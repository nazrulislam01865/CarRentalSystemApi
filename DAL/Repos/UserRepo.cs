using DAL.EF;
using DAL.EF.Models.Entities;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repos
{
    public class UserRepo: IRepository<User>
    {
        UMSContext db;
        public UserRepo(UMSContext db)
        {
            this.db = db;
        }
        public bool Create(User u)
        {
            db.Users.Add(u);
            return db.SaveChanges() > 0;
        }
        public User Get(int id)
        {
            return db.Users.Find(id);
        }
        public List<User> Get()
        {
            return db.Users.ToList();
        }
        public bool Update(User u)
        {
            var ex = Get(u.Id);
            db.Entry(ex).CurrentValues.SetValues(u);
            return db.SaveChanges() > 0;
        }
        public bool Delete(int id)
        {
            var ex = Get(id);
            db.Users.Remove(ex);
            return db.SaveChanges() > 0;
        }
    }
}
