using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockServe.Logic.Service;
using Stockserve.UnitTest.FakeRepository;


namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class TableServiceTests
    {
        [TestMethod]
        public void GetAllTables_ShouldReturnTablesWithCorrectHasActiveOrders()
        {
            // Arrange
            var fakeTableRepository = new FakeTableRepository();
            var fakeOrderDishRepository = new FakeOrderDishRepository();
            var orderDishService = new OrderDishService(fakeOrderDishRepository);
            var tableService = new TableService(fakeTableRepository, orderDishService);

            // Act
            var result = tableService.GetAllTables();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(t => t.Id == 5 && t.HasActiveOrders)); // Has unpaid active dishes
            Assert.IsTrue(result.Any(t => t.Id == 6 && t.HasActiveOrders)); // Also has unpaid active dish
        }
    }
}