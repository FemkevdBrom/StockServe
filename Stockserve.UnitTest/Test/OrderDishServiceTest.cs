using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stockserve.Domain.Dto;
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
    public class OrderDishServiceTest
    {
        private Mock<IOrderDishRepository> _mockOrderDishRepo;
        private Mock<ILogger<OrderDishService>> _mockLogger;
        private OrderDishService _orderDishService;

        [TestInitialize]
        public void Setup()
        {
            _mockOrderDishRepo = new Mock<IOrderDishRepository>();
            _mockLogger = new Mock<ILogger<OrderDishService>>();
            _orderDishService = new OrderDishService(_mockOrderDishRepo.Object,_mockLogger.Object);

        }
        [TestMethod]
        public void GetOrderDishes_ShouldReturnAllOrderDishes()
        {
            // Arrange
            var orderDishes = new List<OrderDishDto>
            {
                new OrderDishDto { OrderId = 1, DishId = 101, Amount = 2 },
                new OrderDishDto { OrderId = 2, DishId = 102, Amount = 1 },
                new OrderDishDto { OrderId = 3, DishId = 103, Amount = 1 },
                new OrderDishDto { OrderId = 4, DishId = 104, Amount = 2 }
            };

            _mockOrderDishRepo.Setup(repo => repo.GetOrderDishes()).Returns(orderDishes);

            // Act
            var result = _orderDishService.GetOrderDishes();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Any(od => od.OrderId == 1 && od.DishId == 101 && od.Amount == 2));
        }

        public void GetOrderDishesForTable_ValidTableId_ShouldReturnGroupedActiveUnpaidDishes()
        {
            // Arrange
            var tableId = 5;
            var orderDishes = new List<OrderDishDto>
            {
                new OrderDishDto { OrderId = 1, DishId = 101, Amount = 2, Status = "Actief"},
                new OrderDishDto { OrderId = 2, DishId = 102, Amount = 1, Status = "Actief" },
                new OrderDishDto { OrderId = 3, DishId = 103, Amount = 1, Status = "Betaald"}
            };

            _mockOrderDishRepo.Setup(repo => repo.GetOrderDishesForTable(tableId)).Returns(orderDishes);

            // Act
            var result = _orderDishService.GetOrderDishesForTable(tableId);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(d => d.DishId == 101 && d.Amount == 2));
            Assert.IsTrue(result.Any(d => d.DishId == 102 && d.Amount == 1));
        }

        [TestMethod]
        public void GetOrderDishesForTable_InvalidTableId_ShouldReturnEmptyList()
        {
            // Arrange
            _mockOrderDishRepo.Setup(repo => repo.GetOrderDishesForTable(999)).Returns(new List<OrderDishDto>());

            // Act
            var result = _orderDishService.GetOrderDishesForTable(999);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void AddOrderDish_ShouldAddDishToRepository()
        {
            // Arrange
            var newDish = new OrderDishDto
            {
                OrderId = 5,
                DishId = 4,
                Amount = 3
            };
            // Act
            _orderDishService.AddOrderDish(newDish);
            // Assert
            _mockOrderDishRepo.Verify(repo => repo.AddOrderDish(newDish), Times.Once);
        }


        [TestMethod]
        public void UpdateOrderDishStatus_ShouldChangeStatusForMatchingDishes()
        {
            // arrange
            int tableId = 5;
            string Status = "Betaald";
            // act
            _orderDishService.UpdateOrderDishStatus(tableId, Status);
            // Assert
            _mockOrderDishRepo.Verify(repo => repo.UpdateOrderDishStatus(tableId, Status), Times.Once);
        }


        [TestMethod]
        [ExpectedException(typeof(OrderDishServiceException))]
        public void GetOrderDishes_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            _mockOrderDishRepo.Setup(repo => repo.GetOrderDishes())
                .Throws(new OrderDishRepositoryException("Database error", new Exception("Inner exception")));

            // Act
            _orderDishService.GetOrderDishes();

            // Assert
            // Verwacht een OrderDishServiceException
        }

        [TestMethod]
        public void GetOrderDishesForTable_ShouldReturnEmptyList_ForNegativeTableId()
        {
            // Arrange
            _mockOrderDishRepo.Setup(repo => repo.GetOrderDishesForTable(-1)).Returns(new List<OrderDishDto>());

            // Act
            var result = _orderDishService.GetOrderDishesForTable(-1);

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void UpdateOrderDishStatus_ShouldHandleConcurrentRequests()
        {
            // Arrange
            int tableId = 5;
            string status = "Betaald";
            _mockOrderDishRepo.Setup(repo => repo.UpdateOrderDishStatus(It.IsAny<int>(), It.IsAny<string>()));

            // Act
            Parallel.For(0, 10, i => { _orderDishService.UpdateOrderDishStatus(tableId, status); });

            // Assert
            _mockOrderDishRepo.Verify(repo => repo.UpdateOrderDishStatus(tableId, status), Times.Exactly(10));
        }

        [TestMethod]
        [ExpectedException(typeof(OrderDishServiceException))]
        public void AddOrderDish_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            var newDish = new OrderDishDto { OrderId = 1, DishId = 1, Amount = 1 };
            _mockOrderDishRepo.Setup(r => r.AddOrderDish(It.IsAny<OrderDishDto>())).Throws(new OrderDishRepositoryException("DB error", new Exception()));
            // Act
            _orderDishService.AddOrderDish(newDish);
        }

        [TestMethod]
        [ExpectedException(typeof(OrderDishServiceException))]
        public void UpdateOrderDishStatus_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            _mockOrderDishRepo.Setup(r => r.UpdateOrderDishStatus(It.IsAny<int>(), It.IsAny<string>())).Throws(new OrderDishRepositoryException("DB error", new Exception()));
            // Act
            _orderDishService.UpdateOrderDishStatus(1, "Betaald");
        }

        [TestMethod]
        public void GetOrderDishes_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _mockOrderDishRepo.Setup(r => r.GetOrderDishes()).Returns(new List<OrderDishDto>());
            // Act
            var result = _orderDishService.GetOrderDishes();
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

    }
}
