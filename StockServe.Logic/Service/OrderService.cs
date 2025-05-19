using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.InterfaceRepository;

namespace StockServe.Logic.Service
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public List<Order> GetAllOrders()
        {
            List<OrderDto> orderDtos = _orderRepository.GetAllOrders();
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

            _orderRepository.AddOrder(orderDto);
            return orderDto.Id;
        }

        public void UpdatePaymentStatus(int tableId, string payStatus)
        {
            _orderRepository.UpdatePaymentStatus(tableId, payStatus);
        }

    }
}



