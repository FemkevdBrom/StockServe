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

namespace Stockserve.UnitTest.Test
{
    [TestClass]
    public class TableServiceTest
    {
        private Mock<ITableRepository> _mockTableRepo;
        private Mock<OrderDishService> _mockOrderDishService;
        private Mock<ILogger<TableService>> _mockLogger;
        private TableService _tableService;

        [TestInitialize]
        public void Setup()
        {
            _mockTableRepo = new Mock<ITableRepository>();
            _mockOrderDishService = new Mock<OrderDishService>(null, null);
            _mockLogger = new Mock<ILogger<TableService>>();
            _tableService = new TableService(_mockTableRepo.Object, _mockOrderDishService.Object, _mockLogger.Object);
        }


        [TestMethod]
        public void GetAllTables_ShouldReturnEmptyList_WhenNoTablesExist()
        {
            // Arrange
            _mockTableRepo.Setup(r => r.GetAllTables()).Returns(new List<TableDto>());

            // Act
            var result = _tableService.GetAllTables();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetAllTables_ShouldThrowException_WhenRepositoryThrows()
        {
            // Arrange
            _mockTableRepo.Setup(r => r.GetAllTables()).Throws(new TableRepositoryException("DB error", new Exception("inner")));

            // Act
            _tableService.GetAllTables();
        }
    }
} 