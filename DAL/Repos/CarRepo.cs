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
        public bool Update(Car c)
        {
            var ex = Get(c.Id);
            db.Entry(ex).CurrentValues.SetValues(c);
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
