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
    public class UserService
    {
        DataAccessFactory factory;
        public UserService(DataAccessFactory factory)
        {
            this.factory = factory;
        }
        public List<UserDTO> All()
        {
            var data = factory.UserData().Get();
            var ret = MapperConfig.GetMapper().Map<List<UserDTO>>(data);
            return ret;

        }
        public UserDTO Get(int id)
        {
            User data = factory.UserData().Get(id);
            UserDTO ret = MapperConfig.GetMapper().Map<UserDTO>(data);
            return ret;
        }
        public bool Create(UserDTO dto)
        {
            User data = MapperConfig.GetMapper().Map<User>(dto);
            return factory.UserData().Create(data);
        }
        public bool Update(UserDTO u)
        {
            User data = MapperConfig.GetMapper().Map<User>(u);
            return factory.UserData().Update(data);

        }
        
        public bool Delete(int id) {
            return factory.UserData().Delete(id);
        
        }
    }
}
