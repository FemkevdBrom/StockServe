using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stockserve.Domain.Dto;
using StockServe.Logic.Service;
using StockServe.Logic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Model;
using Moq;
using StockServe.Logic.InterfaceRepository;
using Microsoft.Extensions.Logging;


namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class OrderServiceTest
    {
        private Mock<IOrderRepository> _mockOrderRepo;
        private Mock<ILogger<OrderService>> _mockLogger;
        private OrderService _orderService;

        [TestInitialize]
        public void Setup()
        {
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _orderService = new OrderService(_mockOrderRepo.Object, _mockLogger.Object);
            
        }
        [TestMethod]
        public void GetAllOrders_ShouldReturnAllOrders()
        {
            // Arrange
            var OrderDtos = new List<OrderDto>
            {
                new OrderDto { Id = 1, TableId = 10, Time = DateTime.Now.AddMinutes(-10), Price = 20.5m, Paystatus = "Nog niet betaald" },
                new OrderDto { Id = 2, TableId = 10, Time = DateTime.Now.AddMinutes(-5), Price = 15.0m, Paystatus = "Betaald Pin" },
                new OrderDto { Id = 3, TableId = 11, Time = DateTime.Now, Price = 30.0m, Paystatus = "Nog niet betaald" },
                new OrderDto { Id = 4, TableId = 12, Time = DateTime.Now.AddMinutes(-2), Price = 25.0m, Paystatus = "Betaald Cash" },
                new OrderDto { Id = 5, TableId = 13, Time = DateTime.Now.AddMinutes(-1), Price = 10.0m, Paystatus = "Nog niet betaald" }
            };
            _mockOrderRepo.Setup(r => r.GetAllOrders()).Returns(OrderDtos);
            // Act
            var orders = _orderService.GetAllOrders();
            // Assert
            Assert.AreEqual(5, orders.Count);
        }

        [TestMethod]
        public void AddOrder_ShouldAddNewOrderAndReturnId()
        {
            // Arrange
            var newOrder = new Order
            {
                TableId = 12,
                Time = DateTime.Now,
                Price = 42.5m,
                Paystatus = "Nog niet betaald"
            };
            var addOrderId = 6; // Simulated ID for the new order
            _mockOrderRepo.Setup(r => r.AddOrder(It.IsAny<OrderDto>())).Callback<OrderDto>(o => o.Id = addOrderId);

            // Act
            int resultId = _orderService.AddOrder(newOrder);

            // Assert
            Assert.AreEqual(addOrderId, resultId);
        }

        [TestMethod]
        public void UpdatePaymentStatus_ShouldUpdateCorrectOrders()
        {
            // Arrange
            int  tableId = 10;
            string Status = "Betaald Pin";
            // Act

            _orderService.UpdatePaymentStatus(tableId, Status);

            // Assert
            _mockOrderRepo.Verify(r => r.UpdatePaymentStatus(tableId, Status), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(OrderServiceException))]
        public void AddOrder_InvalidTableId_ShouldThrowException()
        {
            // Arrange
            var invalidOrder = new Order
            {
                TableId = -1, // Invalid table ID
                Time = DateTime.Now,
                Price = 10.0m,
                Paystatus = "Nog niet betaald"
            };
            // Act
            _orderService.AddOrder(invalidOrder);
            // Assert is handled by ExpectedException
        }
        [TestMethod]
        [ExpectedException(typeof(OrderServiceException))]
        public void AddOrder_InvalidPrice_ShouldThrowException()
        {
            // Arrange
            var invalidOrder = new Order
            {
                TableId = 1,
                Time = DateTime.Now,
                Price = -10.0m, // Invalid price
                Paystatus = "Nog niet betaald"
            };
            // Act
            _orderService.AddOrder(invalidOrder);
            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(OrderServiceException))]
        public void AddOrder_RepositoryThrowsException_ShouldThrowServiceException()
        {
            // Arrange
            var validOrder = new Order
            {
                TableId = 1,
                Time = DateTime.Now,
                Price = 10.0m,
                Paystatus = "Nog niet betaald"
            };
            _mockOrderRepo.Setup(r => r.AddOrder(It.IsAny<OrderDto>())).Throws(new OrderRepositoryException("Repository error", new Exception()));
            // Act
            _orderService.AddOrder(validOrder);
            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(OrderServiceException))]
        public void GetAllOrders_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            _mockOrderRepo.Setup(repo => repo.GetAllOrders())
                .Throws(new OrderRepositoryException("Database error", new Exception("Inner exception")));

            // Act
            _orderService.GetAllOrders();

            // Assert
            // Verwacht een OrderServiceException
        }

        [TestMethod]
        public void GetAllOrders_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.GetAllOrders()).Returns(new List<OrderDto>());
            // Act
            var result = _orderService.GetAllOrders();
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(OrderServiceException))]
        public void UpdatePaymentStatus_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            _mockOrderRepo.Setup(r => r.UpdatePaymentStatus(It.IsAny<int>(), It.IsAny<string>())).Throws(new OrderRepositoryException("DB error", new Exception()));
            // Act
            _orderService.UpdatePaymentStatus(1, "Betaald");
        }

    }

}