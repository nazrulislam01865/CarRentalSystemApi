using BLL.DTOs;
using DAL;
using DAL.EF.Models.Entities;
using DAL.EF.Models.Enums;
using DAL.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class BookingService
    {
        readonly DataAccessFactory factory;
        public BookingService(DataAccessFactory factory)
        {
            this.factory = factory;
        }
        public List<BookingDTO> All()
        {
            var data = factory.BookingData().Get();
            return MapperConfig.GetMapper().Map<List<BookingDTO>>(data);
        }

        public BookingDTO Get(int id)
        {
            var data = factory.BookingData().Get(id);
            return MapperConfig.GetMapper().Map<BookingDTO>(data);
        }

        public bool Create(BookingDTO dto)
        {
            if (dto.CarId <= 0 || dto.CustomerId <= 0)
                return false;

            var start = dto.StartDate.Date;
            var end = dto.EndDate.Date;
            if (end <= start)
                return false;

            var car = factory.CarData().Get(dto.CarId);
            if (car == null) return false;
            if (car.Status == CarStatus.Maintenance) return false;


            dto.Status = BookingStatus.Pending;
            dto.ApprovedById = null;

            dto.TotalAmount = (decimal)(dto.EndDate - dto.StartDate).TotalDays * car.Deposit; ;
            var entity = MapperConfig.GetMapper().Map<Booking>(dto);
            return factory.BookingData().Create(entity);
            

            
        }
    }
}
