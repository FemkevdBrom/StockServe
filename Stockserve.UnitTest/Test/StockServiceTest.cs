using Moq;
using Stockserve.Domain.Model;
using StockServe.Logic.Exceptions;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class StockServiceTests
    {
        private Mock<IStockRepository> _mockStockRepo;
        private Mock<ILogger<StockService>> _mockLogger;
        private StockService _stockService;

        [TestInitialize]
        public void Setup()
        {
            _mockStockRepo = new Mock<IStockRepository>();
            _mockLogger = new Mock<ILogger<StockService>>();
            _stockService = new StockService(_mockStockRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetAllStocksAsync_ShouldReturnMappedStockDtos()
        {
            // Arrange
            var stocks = new List<Stock>
            {
                new Stock { Id = 1, Name = "Aardappels", StockQuantity = 10, MinimumStock = 5, DesiredStock = 20, OrderedStock = 0, Supplier = "SupplierA", SupplierValue = 5 },
                new Stock { Id = 2, Name = "Ui", StockQuantity = 3, MinimumStock = 5, DesiredStock = 10, OrderedStock = 0, Supplier = "SupplierB", SupplierValue = 2 }
            };
            _mockStockRepo.Setup(repo => repo.GetAllStocksAsync()).ReturnsAsync(stocks);

            // Act
            var result = await _stockService.GetAllStocksAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Aardappels", result[0].Name);
        }

        [TestMethod]
        public async Task GetOrderListAsync_ShouldFilterCorrectly()
        {
            // Arrange
            var stocks = new List<Stock>
            {
                new Stock { Id = 1, Name = "Bier", StockQuantity = 2, MinimumStock = 5 },
                new Stock { Id = 2, Name = "Wijn", StockQuantity = 7, MinimumStock = 5 }
            };
            _mockStockRepo.Setup(r => r.GetAllStocksAsync()).ReturnsAsync(stocks);

            // Act
            var result = await _stockService.GetOrderListAsync();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Bier", result[0].Name);
        }

        [TestMethod]
        public async Task GetDeliveredListAsync_ShouldReturnOnlyOrderedBelowMinimum()
        {
            // Arrange
            var stocks = new List<Stock>
            {
                new Stock { Id = 1, Name = "Fris", StockQuantity = 3, MinimumStock = 5, OrderedStock = 10 },
                new Stock { Id = 2, Name = "Water", StockQuantity = 10, MinimumStock = 5, OrderedStock = 5 },
                new Stock { Id = 3, Name = "Bier", StockQuantity = 2, MinimumStock = 5, OrderedStock = 0 }
            };
            _mockStockRepo.Setup(r => r.GetAllStocksAsync()).ReturnsAsync(stocks);

            // Act
            var result = await _stockService.GetDeliveredListAsync();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Fris", result[0].Name);
        }

        [TestMethod]
        public async Task UpdateBestellingAsync_ShouldUpdate_WhenValid()
        {
            // Arrange
            var stock = new Stock { Id = 1, SupplierValue = 5, OrderedStock = 0 };
            _mockStockRepo.Setup(r => r.GetAllStocksAsync()).ReturnsAsync(new List<Stock> { stock });

            // Act
            await _stockService.UpdateBestellingAsync(1, 10);

            // Assert
            _mockStockRepo.Verify(r => r.UpdateOrderedStockAsync(1, 10), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(StockServiceException))]
        public async Task UpdateBestellingAsync_ShouldThrow_WhenNotMultipleOfSupplierValue()
        {
            // Arrange
            var stock = new Stock { Id = 1, SupplierValue = 5 };
            _mockStockRepo.Setup(r => r.GetAllStocksAsync()).ReturnsAsync(new List<Stock> { stock });

            // Act
            await _stockService.UpdateBestellingAsync(1, 3);
        }

        [TestMethod]
        public async Task ProcessDeliveredItemsAsync_ShouldUpdateQuantitiesCorrectly()
        {
            // Arrange
            var stock = new Stock { Id = 1, StockQuantity = 5, OrderedStock = 10 };
            _mockStockRepo.Setup(r => r.GetAllStocksAsync()).ReturnsAsync(new List<Stock> { stock });

            // Act
            await _stockService.ProcessDeliveredItemsAsync(new List<int> { 1 });

            // Assert
            _mockStockRepo.Verify(r => r.UpdateStockQuantityAsync(1, 15), Times.Once);
            _mockStockRepo.Verify(r => r.UpdateOrderedStockAsync(1, 0), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStockQuantityAsync_ShouldUpdateCorrectly()
        {
            // Arrange
            var stock = new Stock { Id = 1, StockQuantity = 5 };
            _mockStockRepo.Setup(r => r.GetAllStocksAsync()).ReturnsAsync(new List<Stock> { stock });

            // Act
            await _stockService.UpdateStockQuantityAsync(1, 20);

            // Assert
            _mockStockRepo.Verify(r => r.UpdateStockQuantityAsync(1, 20), Times.Once);
        }
    }
}
