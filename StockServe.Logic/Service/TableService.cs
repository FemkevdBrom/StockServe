using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.Exceptions;
using StockServe.Logic.InterfaceRepository;
using Microsoft.Extensions.Logging;

namespace StockServe.Logic.Service
{
    public class TableService
    {
        private readonly OrderDishService _orderDishService;
        private readonly ITableRepository _tableRepository;
        private readonly ILogger<TableService> _logger;

        public TableService(ITableRepository tableRepository, OrderDishService orderDishService, ILogger<TableService> logger)
        {
            _tableRepository = tableRepository;
            _orderDishService = orderDishService;
            _logger = logger;
        }


        public List<Table> GetAllTables()
        {
            try
            {
                List<TableDto> tableDtos = _tableRepository.GetAllTables();
                List<Table> tables = new List<Table>();
                foreach (var tableDto in tableDtos)
                {
                    var orderDishes = _orderDishService.GetOrderDishesForTable(tableDto.Id);
                    tables.Add(new Table
                    {
                        Id = tableDto.Id,
                        TableNumber = tableDto.TableNumber,
                        HasActiveOrders = orderDishes.Any()
                    });
                }
                return tables;
            }
            catch (TableRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van alle tafels in de repository");
                // Vang specifieke repository fouten op
                throw new Exception("Fout bij ophalen van alle tafels.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van alle tafels in de service");
                throw new Exception("Fout bij ophalen van alle tafels.", ex);
            }
        }
    }
}
