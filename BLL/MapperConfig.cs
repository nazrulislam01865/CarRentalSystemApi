using AutoMapper;
using BLL.DTOs;
using DAL.EF.Models.Entities;

namespace BLL
{
    public class MapperConfig
    {
        static MapperConfiguration cfg = new MapperConfiguration(c => {
            c.CreateMap<User, UserDTO>().ReverseMap();
            c.CreateMap<Car, CarDTO>().ReverseMap();
            c.CreateMap<Booking, BookingDTO>().ReverseMap();
            //c.CreateMap<Department, DepartmentDTO>().ReverseMap();
            //c.CreateMap<Student, StudentDTO>().ReverseMap();
            //c.CreateMap<Department, DepartmentStudentDTO>().ReverseMap();
            //c.CreateMap<Department, DepartmentCountDTO>().ForMember(
            //        dto => dto.Count,
            //        src => src.MapFrom(d => d.Students.Count)
            //);
            //c.CreateMap<Department, DepartmentDTO>().ReverseMap();
            //c.CreateMap<Department, DepartmentDTO>().ReverseMap();

        });
        public static Mapper GetMapper()
        {
            return new Mapper(cfg);
        }

    }
}
