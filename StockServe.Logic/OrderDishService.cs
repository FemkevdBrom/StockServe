using StockServe.Data;
using StockServe.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic
{
    public class OrderDishService
    {
        public List<OrderDish> GetOrderDishes()
        {
            List<OrderDishDto> orderDishDtos = new OrderDishRepository().GetOrderDishes();
            List<OrderDish> orderDishes = new List<OrderDish>();
            foreach (var orderDishDto in orderDishDtos)
            {
                orderDishes.Add(new OrderDish
                {
                    OrderId = orderDishDto.OrderId,
                    DishId = orderDishDto.DishId,
                    Amount = orderDishDto.Amount
                });
            }
            return orderDishes;
        }

        public List<OrderDish> GetOrderDishesForTable(int tableId)
        {
            List<OrderDishDto> orderDishDtos = new OrderDishRepository().GetOrderDishesForTable(tableId);
            List<OrderDish> orderDishes = new List<OrderDish>();
            foreach (var orderDishDto in orderDishDtos)
            {
                orderDishes.Add(new OrderDish
                {
                    OrderId = orderDishDto.OrderId,
                    DishId = orderDishDto.DishId,
                    Amount = orderDishDto.Amount
                });
            }
            return orderDishes;
        }
    }
}
