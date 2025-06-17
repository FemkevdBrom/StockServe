using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.Exceptions;
using StockServe.Logic.InterfaceRepository;
using Microsoft.Extensions.Logging;


namespace StockServe.Logic.Service
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
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
                _logger.LogError(ex, "Fout bij het controleren van de gebruiker in de repository");
                throw new Exception("Een foutmelding tijdens het controleren van de gebruiker .", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het controleren van de gebruiker in de service");
                throw new Exception("een foutemlding tijdens het controleren van de gebruiker.", ex);
            }
        }
    }
}