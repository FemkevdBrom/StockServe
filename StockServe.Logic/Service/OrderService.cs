using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;


namespace StockServe.Logic.Service
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
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
                _logger.LogError(ex, "Fout bij het ophalen van alle bestellingen in de repository");
                // Vang specifieke repository fouten op
                throw new OrderServiceException("Fout bij ophalen van alle bestellingen.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van alle bestellingen in de service");
                // Vang overige onverwachte fouten op
                throw new OrderServiceException("Onverwachte fout bij service.", ex);

            }
        }


        public int AddOrder(Order order)
        {
            try
            { 
                ValidateOrder(order);
                
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
            catch (OrderRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het toevoegen van een bestelling in de repository");
                // Vang specifieke repository fouten op
                throw new OrderServiceException("Fout bij toevoegen van bestelling.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het toevoegen van een bestelling in de service");
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
                _logger.LogError(ex, "Fout bij het bijwerken van de betalingsstatus in de repository");
                // Vang specifieke repository fouten op
                throw new OrderServiceException("Fout bij bijwerken van betalingsstatus.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het bijwerken van de betalingsstatus in de service");
                // Vang overige onverwachte fouten op
                throw new OrderServiceException("Onverwachte fout bij service.", ex);
            }
        }

        private void ValidateOrder(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order), "Order mag niet null zijn.");
            }
            if (order.TableId <= 0)
            {
                throw new ArgumentException("TableId moet groter zijn dan 0.", nameof(order.TableId));
            }
            if (order.Price <= 0)
            {
                throw new ArgumentException("Price mag niet nul of negatief zijn.", nameof(order.Price));
            }
            if (string.IsNullOrEmpty(order.Paystatus))
            {
                throw new ArgumentException("Paystatus mag niet leeg zijn.", nameof(order.Paystatus));
            }

        }
    }
}



