using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.Interface;

namespace StockServe.Logic.Service
{
    public class UserService
    {
        private readonly IUser _userRepository;
        public UserService(IUser userRepository)
        {
            _userRepository = userRepository;
        }

        public User? Authenticate(string email, string password)
        {
            UserDto? dto = _userRepository.GetUserEmailAndPassword(email, password);

            if (dto == null) return null;

            return new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                EmployeeCode = dto.EmployeeCode,
                Role = dto.Role
            };
        }
    }
}
