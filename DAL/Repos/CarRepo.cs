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
    public class CarRepo:IRepository<Car>
    {
        UMSContext db;
        public CarRepo(UMSContext db)
        {
            this.db = db;
        }
        public bool Create(Car c)
        {   
            if(c.PlateNumber == db.Cars.FirstOrDefault().PlateNumber) return (false);
            db.Cars.Add(c);
            return db.SaveChanges() > 0;
        }
        public Car Get(int id)
        {
            return db.Cars.Find(id);
        }
        public List<Car> Get()
        {
            return db.Cars.ToList();
        }

        public bool Update(Car u)
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
            db.Cars.Remove(ex);
            return db.SaveChanges() > 0;
        }
    }
}
