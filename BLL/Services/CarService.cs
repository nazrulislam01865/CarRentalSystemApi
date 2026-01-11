using BLL.DTOs;
using DAL;
using DAL.EF.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CarService
    {
        DataAccessFactory factory;
        public CarService(DataAccessFactory factory)
        {
            this.factory = factory;
        }

        public List<CarDTO> All() { 
            var data = factory.CarData().Get();
            var ret = MapperConfig.GetMapper().Map<List<CarDTO>>(data);
            return ret;
        }
        public CarDTO Get(int id)
        {
            Car data = factory.CarData().Get(id);
            CarDTO ret = MapperConfig.GetMapper().Map<CarDTO>(data);
            return ret;

        }
        public bool Create(CarDTO dto)
        {
            Car data = MapperConfig.GetMapper().Map<Car>(dto);
            return factory.CarData().Create(data);
        }
        public bool Update(CarDTO c)
        {
            Car data = MapperConfig.GetMapper().Map<Car>(c);
            return factory.CarData().Update(data);
        }
        public bool Delete(int id)
        {
            return factory.CarData().Delete(id);
        }

    }
}
