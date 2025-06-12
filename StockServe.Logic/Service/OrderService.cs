using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
            try
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
            catch (OrderRepositoryException ex)
            {
                // Vang specifieke repository fouten op
                throw new OrderServiceException("Fout bij ophalen van alle bestellingen.", ex);
            }
            catch (Exception ex)
            {
                // Vang overige onverwachte fouten op
                throw new OrderServiceException("Onverwachte fout bij service.", ex);

            }
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

            try
            {
                _orderRepository.AddOrder(orderDto);
                return orderDto.Id;
            }
            catch (OrderRepositoryException ex)
            {
                // Vang specifieke repository fouten op
                throw new OrderServiceException("Fout bij toevoegen van bestelling.", ex);
            }
            catch (Exception ex)
            {
                // Vang overige onverwachte fouten op
                throw new OrderServiceException("Onverwachte fout bij service.", ex);
            }
        }

        public void UpdatePaymentStatus(int tableId, string payStatus)
        {
            try
            {
                _orderRepository.UpdatePaymentStatus(tableId, payStatus);
            }
            catch (OrderRepositoryException ex)
            {
                // Vang specifieke repository fouten op
                throw new OrderServiceException("Fout bij bijwerken van betalingsstatus.", ex);
            }
            catch (Exception ex)
            {
                // Vang overige onverwachte fouten op
                throw new OrderServiceException("Onverwachte fout bij service.", ex);
            }
        }

    }
}



