using StockServe.Logic.Service;
using Stockserve.UnitTest.FakeRepository;

namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class DishServiceTest
    {
        private DishService _dishService;
        [TestInitialize]
        public void Setup()
        {
            var fakeRepo = new FakeDishRepository();
            _dishService = new DishService(fakeRepo);
        }
        [TestMethod]
        public void GetAllDishes_ShouldReturnAllDishes()
        {
            // Act
            var result = _dishService.GetAllDishes();
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);
        }
        [TestMethod]
        public void GetAllDishes_DoesNotReturnNull()
        {
            // Act
            var result = _dishService.GetAllDishes();
            // Assert
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void DishExists_ShouldReturnTrue_WhenDishExists()
        {
            // Act
            var result = _dishService.DishExists(1);
            // Assert
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void DishExists_ShouldReturnFalse_WhenDishDoesNotExist()
        {
            // Act
            var result = _dishService.DishExists(10);
            // Assert
            Assert.IsFalse(result);
        }

    }
}
