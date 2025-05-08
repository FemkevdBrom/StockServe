using StockServe.Data;
using StockServe.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic
{
    public class OrderService
    {
        public List<Order> GetAllOrders()
        {
            List<OrderDto> orderDtos = new OrderRepository().GetAllOrders();
            List<Order> orders = new List<Order>();
            foreach (var orderDto in orderDtos)
            {
                orders.Add(new Order
                {
                    Id = orderDto.Id,
                    TableId = orderDto.TableId,
                    Time = orderDto.Time,
                    Price = orderDto.Price,
                    Paystatus = orderDto.Paystatus
                });
            }
            return orders;
        }

        public int AddOrder(Order order)
        {
            var orderDto = new OrderDto
            {
                TableId = order.TableId,
                Time = order.Time,
                Price = order.Price,
                Paystatus = order.Paystatus
            };

            new OrderRepository().AddOrder(orderDto);
            return orderDto.Id;
        }

        public void UpdatePaymentStatus(int tableId, string payStatus)
        {
            new OrderRepository().UpdatePaymentStatus(tableId, payStatus);
        }
    }
}



