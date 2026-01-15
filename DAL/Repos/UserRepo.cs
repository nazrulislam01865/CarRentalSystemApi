using Azure;
using DAL.EF;
using DAL.EF.Models.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            return db.Users.FirstOrDefault(x => x.Id == id); ;
        }
        public List<User> Get()
        {
            return db.Users.ToList();
        }
        //public bool Update(User u)
        //{
        //    var ex = Get(u.Id);
        //    if (ex == null) {
        //        return true;
        //    }
        //    db.Entry(u).CurrentValues.SetValues(u);
        //    return db.SaveChanges() > 0;
        //}
        //public bool Update(User u)
        //{
        //    if (u == null || u.Id <= 0)
        //        return false;
        //    bool exists = db.Users.Any(x => x.Id == u.Id);
        //    if (!exists)
        //        return false;

        //    db.Users.Attach(u);
        //    db.Entry(u).State = EntityState.Modified;

        //    return db.SaveChanges() > 0;
        //}
        public bool Update(User u)
        {
            if (u == null || u.Id <= 0)
                return false;

            var ex = Get(u.Id);
            if (ex == null)
                return false; 

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
