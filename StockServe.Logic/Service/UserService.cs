using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.Exceptions;
using StockServe.Logic.InterfaceRepository;

namespace StockServe.Logic.Service
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User? Authenticate(string email, string password)
        {
            try
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
            catch (UserRepositoryException ex)
            {
                throw new Exception("An error occurred while retrieving user data.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while authenticating the user.", ex);
            }
        }
    }
}