using StockServe.Logic.Service;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;

namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class DishServiceTest
    {
        private Mock<IDishRepository> _mockDishRepository;
        private DishService _dishService;
        [TestInitialize]
        public void Setup()
        {
            _mockDishRepository = new Mock<IDishRepository>();
            _dishService = new DishService(_mockDishRepository.Object);
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

    }
}
