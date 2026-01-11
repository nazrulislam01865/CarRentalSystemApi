using DAL.EF;
using DAL.EF.Models.Entities;
using DAL.Interfaces;
using DAL.Repos;

namespace DAL
{
    public class DataAccessFactory
    {

        UMSContext db;
        public DataAccessFactory(UMSContext db)
        {
            this.db = db;
        }

        public IRepository<User> UserData()
        {
            return new UserRepo(db);
        }
        public IRepository<Car> CarData()
        {
            return new CarRepo(db);
        }
    }
}
