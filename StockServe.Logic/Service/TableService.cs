using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.Interface;

namespace StockServe.Logic.Service
{
    public class TableService
    {
        private readonly OrderDishService _orderDishService;
        private readonly ITable _tableRepository;
        public TableService(ITable tableRepository, OrderDishService orderDishService)
        {
            _tableRepository = tableRepository;
            _orderDishService = orderDishService;
        }


        public List<Table> GetAllTables()
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
    }
}
