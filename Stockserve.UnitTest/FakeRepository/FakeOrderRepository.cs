using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;


namespace Stockserve.UnitTest.FakeRepository
{
    public class FakeOrderRepository : IOrderRepository
    {
        private readonly List<OrderDto> _orders = new List<OrderDto>
        {
            new OrderDto { Id = 1, TableId = 10, Time = DateTime.Now.AddMinutes(-10), Price = 20.5m, Paystatus = "Nog niet betaald" },
            new OrderDto { Id = 2, TableId = 10, Time = DateTime.Now.AddMinutes(-5), Price = 15.0m, Paystatus = "Betaald Pin" },
            new OrderDto { Id = 3, TableId = 11, Time = DateTime.Now, Price = 30.0m, Paystatus = "Nog niet betaald" },
            new OrderDto { Id = 4, TableId = 12, Time = DateTime.Now.AddMinutes(-2), Price = 25.0m, Paystatus = "Betaald Cash" },
            new OrderDto { Id = 5, TableId = 13, Time = DateTime.Now.AddMinutes(-1), Price = 10.0m, Paystatus = "Nog niet betaald" }
        };

        public List<OrderDto> GetAllOrders()
        {
            return _orders.Select(o => new OrderDto
            {
                Id = o.Id,
                TableId = o.TableId,
                Time = o.Time,
                Price = o.Price,
                Paystatus = o.Paystatus
            }).ToList();
        }

        public void AddOrder(OrderDto order)
        {
            order.Id = _orders.Max(o => o.Id) + 1;
            _orders.Add(order);
        }

        public void UpdatePaymentStatus(int tableId, string payStatus)
        {
            foreach (var order in _orders.Where(o => o.TableId == tableId && o.Paystatus == "Nog niet betaald"))
            {
                order.Paystatus = payStatus;
            }
        }
    }
}
