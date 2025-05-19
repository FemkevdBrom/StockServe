using Stockserve.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic.InterfaceRepository
{
    public interface IOrderDishRepository
    {
        List<OrderDishDto> GetOrderDishes();
        List<OrderDishDto> GetOrderDishesForTable(int tableId);
        void AddOrderDish(OrderDishDto orderDish);
        void UpdateOrderDishStatus(int tableId, string status);
    }
}
