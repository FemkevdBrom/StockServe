using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.Exceptions;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class UserServiceTest
    {
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<ILogger<UserService>> _mockLogger;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public void Authenticate_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var validUserDto = new UserDto
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Password = "password123",
                Role = "Admin"
            };

            _mockUserRepo.Setup(repo => repo.GetUserEmailAndPassword("john@example.com", "password123")).Returns(validUserDto);

            // Act
            var result = _userService.Authenticate("john@example.com", "password123");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(validUserDto.Email, result.Email);
        }

        [TestMethod]
        public void Authenticate_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetUserEmailAndPassword("invalid@example.com", "wrongpassword")).Returns((UserDto)null);

            // Act
            var result = _userService.Authenticate("invalid@example.com", "wrongpassword");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Authenticate_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetUserEmailAndPassword(It.IsAny<string>(), It.IsAny<string>())).Throws(new UserRepositoryException("Database error", new Exception("Inner exception")));

            // Act
            _userService.Authenticate("user@example.com", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Authenticate_ShouldThrowGeneralException_WhenOtherExceptionOccurs()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetUserEmailAndPassword(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("General error"));

            // Act
            _userService.Authenticate("user@example.com", "password");
        }

        [TestMethod]
        public void Authenticate_ShouldReturnNull_WhenEmailOrPasswordIsNull()
        {
            // Arrange
            _mockUserRepo.Setup(repo => repo.GetUserEmailAndPassword(null, null)).Returns((UserDto)null);

            // Act
            var result = _userService.Authenticate(null, null);

            // Assert
            Assert.IsNull(result);
        }
    }
}
