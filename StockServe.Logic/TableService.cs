using StockServe.Data;

namespace StockServe.Logic
{
    public class TableService
    {
        private readonly OrderDishService _orderDishService;

        public TableService(OrderDishService orderDishService)
        {
            _orderDishService = orderDishService;
        }

        public List<Table> GetAllTables()
        {
            List<TableDto> tableDtos = new TableRepository().GetAllTables();
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
