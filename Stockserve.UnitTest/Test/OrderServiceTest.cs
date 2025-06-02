using StockServe.Logic.Service;
using Stockserve.UnitTest.FakeRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Model;

namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class OrderServiceTest
    {
        private OrderService _orderService;

        [TestInitialize]
        public void Setup()
        {
            var fakeRepo = new FakeOrderRepository();
            _orderService = new OrderService(fakeRepo);
        }

        [TestMethod]
        public void GetAllOrders_ShouldReturnAllOrders()
        {
            // Act
            var orders = _orderService.GetAllOrders();
            // Assert
            Assert.IsNotNull(orders);
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

            // Act
            int newId = _orderService.AddOrder(newOrder);
            var result = _orderService.GetAllOrders();

            // Assert
            Assert.IsTrue(result.Any(o => o.Id == newId && o.TableId == 12 && o.Price == 42.5m));
        }

        [TestMethod]
        public void UpdatePaymentStatus_ShouldUpdateCorrectOrders()
        {
            // Act
            _orderService.UpdatePaymentStatus(10, "Betaald Cash");

            // Assert
            var result = _orderService.GetAllOrders();
            var table10Orders = result.Where(o => o.TableId == 10).ToList();

            Assert.AreEqual("Betaald Cash", table10Orders.First(o => o.Id == 1).Paystatus);
            Assert.AreEqual("Betaald Pin", table10Orders.First(o => o.Id == 2).Paystatus); // already was "Betaald"
        }
    }
}