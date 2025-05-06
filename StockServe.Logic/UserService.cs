using StockServe.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic
{
    public class UserService
    {
        private UserRepository _userRepository = new UserRepository();

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
