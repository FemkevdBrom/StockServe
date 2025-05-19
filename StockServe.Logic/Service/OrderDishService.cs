using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Model;
using Stockserve.Domain.Dto;
using StockServe.Logic.Interface;

namespace StockServe.Logic.Service
{
    public class OrderDishService
    {
        public readonly IOrderDish _orderDishRepository;
        public OrderDishService(IOrderDish orderDishRepository)
        {
            _orderDishRepository = orderDishRepository;
        }
        public List<OrderDish> GetOrderDishes()
        {
            List<OrderDishDto> orderDishDtos = _orderDishRepository.GetOrderDishes();
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
            List<OrderDishDto> orderDishDtos = _orderDishRepository.GetOrderDishesForTable(tableId);
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
        public void AddOrderDish(OrderDishDto orderDish)
        {
            _orderDishRepository.AddOrderDish(orderDish);
        }

        public void UpdateOrderDishStatus(int tableId, string status)
        {
            _orderDishRepository.UpdateOrderDishStatus(tableId, status);
        }
    }
}
