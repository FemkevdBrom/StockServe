using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockserve.UnitTest.FakeRepository
{
    public class FakeOrderDishRepository : IOrderDishRepository
    {
        // Simulate database with in-memory list
        private readonly List<OrderDishDto> _orderDishes = new List<OrderDishDto>
    {
        new OrderDishDto { OrderId = 1, DishId = 101, Amount = 2, Status = "Actief" },
        new OrderDishDto { OrderId = 2, DishId = 102, Amount = 1, Status = "Actief" },
        new OrderDishDto { OrderId = 3, DishId = 101, Amount = 3, Status = "Betaald" },
        new OrderDishDto { OrderId = 4, DishId = 103, Amount = 1, Status = "Actief" }
    };

        private readonly List<OrderDto> _orders = new List<OrderDto>
    {
        new OrderDto { Id = 1, TableId = 5, Paystatus = "Nog niet betaald" },
        new OrderDto { Id = 2, TableId = 5, Paystatus = "Nog niet betaald" },
        new OrderDto { Id = 3, TableId = 5, Paystatus = "Betaald" },
        new OrderDto { Id = 4, TableId = 6, Paystatus = "Nog niet betaald" }
    };

        public List<OrderDishDto> GetOrderDishes()
        {
            return _orderDishes.Select(d => new OrderDishDto
            {
                OrderId = d.OrderId,
                DishId = d.DishId,
                Amount = d.Amount,
                Status = d.Status
            }).ToList();
        }

        public List<OrderDishDto> GetOrderDishesForTable(int tableId)
        {
            // Find active OrderDishes linked to unpaid orders for the table
            var validOrderIds = _orders
                .Where(o => o.TableId == tableId && o.Paystatus == "Nog niet betaald")
                .Select(o => o.Id)
                .ToList();

            return _orderDishes
                .Where(od => validOrderIds.Contains(od.OrderId) && od.Status == "Actief")
                .GroupBy(od => od.DishId)
                .Select(g => new OrderDishDto
                {
                    DishId = g.Key,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToList();
        }

        public void AddOrderDish(OrderDishDto orderDish)
        {
            // Add to the fake list
            _orderDishes.Add(new OrderDishDto
            {
                OrderId = orderDish.OrderId,
                DishId = orderDish.DishId,
                Amount = orderDish.Amount,
                Status = "Actief"
            });
        }

        public void UpdateOrderDishStatus(int tableId, string status)
        {
            // Find order IDs for the table that are unpaid
            var validOrderIds = _orders
                .Where(o => o.TableId == tableId && o.Paystatus == "Nog niet betaald")
                .Select(o => o.Id)
                .ToList();

            // Update status
            foreach (var od in _orderDishes
                         .Where(od => validOrderIds.Contains(od.OrderId) && od.Status == "Actief"))
            {
                od.Status = status;
            }
        }
    }
}
