using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stockserve.Domain.Dto;
using StockServe.Logic.Service;
using Stockserve.UnitTest.FakeRepository;


namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class OrderDishServiceTest
    {
        private OrderDishService _orderDishService;
        private FakeOrderDishRepository _fakeRepo;

        [TestInitialize]
        public void Setup()
        {
            _fakeRepo = new FakeOrderDishRepository();
            _orderDishService = new OrderDishService(_fakeRepo);
        }
        [TestMethod]
        public void GetOrderDishes_ShouldReturnAllOrderDishes()
        {
            // Act
            var result = _orderDishService.GetOrderDishes();
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.Any(od => od.OrderId == 1 && od.DishId == 101 && od.Amount == 2));
        }

        public void GetOrderDishesForTable_ValidTableId_ShouldReturnGroupedActiveUnpaidDishes()
        {
            //act
            var result = _orderDishService.GetOrderDishesForTable(5);
            //assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(d => d.DishId == 101 && d.Amount == 2));
            Assert.IsTrue(result.Any(d => d.DishId == 102 && d.Amount == 1));
        }

        [TestMethod]
        public void GetOrderDishesForTable_InvalidTableId_ShouldReturnEmptyList()
        {
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
            var result = _orderDishService.GetOrderDishes();
            Assert.IsTrue(result.Any(d => d.OrderId == 5 && d.DishId == 4 && d.Amount == 3));
        }

        [TestMethod]
        public void UpdateOrderDishStatus_ShouldChangeStatusForMatchingDishes()
        {
            // arrange
            _orderDishService.UpdateOrderDishStatus(5, "Betaald");
            // act
            var updated = _fakeRepo
                .GetOrderDishes()
                .Where(od => (od.OrderId == 1 || od.OrderId == 2) && od.Status == "Betaald")
                .ToList();
            // Assert
            Assert.AreEqual(2, updated.Count);
        }
    }
}
