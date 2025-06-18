using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stockserve.Domain.Dto;
using StockServe.Logic.Exceptions;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Service;
using Stockserve.Domain.Model;

namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class DishServiceTest
    {
        private Mock<IDishRepository> _mockDishRepository;
        private Mock<ILogger<DishService>> _mockLogger;
        private DishService _dishService;
        [TestInitialize]
        public void Setup()
        {
            _mockDishRepository = new Mock<IDishRepository>();
            _mockLogger = new Mock<ILogger<DishService>>();
            _dishService = new DishService(_mockDishRepository.Object, _mockLogger.Object);
        }

        [TestMethod]
        public void GetAllDishes_ShouldReturnAllDishes()
        {
            // Arrange
            var dishlist = new List<DishDto>
            {
                new DishDto { Id = 1, Name = "Pasta", Price = 12.99m, Category = "Main Course", Description = "Delicious pasta with tomato sauce" },
                new DishDto { Id = 2, Name = "Salad", Price = 8.99m, Category = "Appetizer", Description = "Fresh garden salad" },
                new DishDto { Id = 3, Name = "Pizza", Price = 10.99m, Category = "Main Course", Description = "Cheesy pizza with various toppings" },
                new DishDto { Id = 4, Name = "Ice Cream", Price = 5.99m, Category = "Dessert", Description = "Creamy vanilla ice cream" }
            };

            _mockDishRepository.Setup(repo => repo.GetAllDishes()).Returns(dishlist);
            // Act
            var result = _dishService.GetAllDishes();
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
        }
        [TestMethod]
        public void GetAllDishes_DoesNotReturnNull()
        {
            // Arrange
            _mockDishRepository.Setup(repo => repo.GetAllDishes()).Returns(new List<DishDto>());
            // Act
            var result = _dishService.GetAllDishes();
            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void DishExists_ShouldReturnTrue_WhenDishExists()
        {
            // Arrange
            int dishId = 1;
            _mockDishRepository.Setup(repo => repo.DishExists(dishId)).Returns(true);
            // Act
            var result = _dishService.DishExists(1);
            // Assert
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void DishExists_ShouldReturnFalse_WhenDishDoesNotExist()
        {
            // Arrange
            int dishId = 10;
            _mockDishRepository.Setup(repo => repo.DishExists(dishId)).Returns(false);
            // Act
            var result = _dishService.DishExists(10);
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DishServiceException))]
        public void GetAllDishes_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            _mockDishRepository.Setup(repo => repo.GetAllDishes()).Throws(new DishRepositoryException("Database error", new Exception("Inner exception")));

            // Act
            _dishService.GetAllDishes();

            // Assert
            // Verwacht een DishServiceException
        }

        [TestMethod]
        public void DishExists_ShouldWorkCorrectly_UnderParallelExecution()
        {
            // Arrange
            int dishId = 1;
            _mockDishRepository.Setup(repo => repo.DishExists(dishId)).Returns(true);

            // Act
            bool[] results = new bool[10];
            Parallel.For(0, 10, i => { results[i] = _dishService.DishExists(dishId); });

            // Assert
            Assert.IsTrue(results.All(r => r == true));
        }

        [TestMethod]
        public void DishExists_ShouldReturnFalse_ForNegativeId()
        {
            // Arrange
            _mockDishRepository.Setup(repo => repo.DishExists(-1)).Returns(false);

            // Act
            var result = _dishService.DishExists(-1);

            // Assert
            Assert.IsFalse(result);
        }


    }
}
