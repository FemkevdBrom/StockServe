using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Model;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;
using Microsoft.Extensions.Logging;

namespace StockServe.Logic.Service
{
    public class OrderDishService
    {
        public readonly IOrderDishRepository _orderDishRepository;
        private readonly ILogger<OrderDishService> _logger;
        public OrderDishService(IOrderDishRepository orderDishRepository, ILogger<OrderDishService> logger)
        {
            _orderDishRepository = orderDishRepository;
            _logger = logger;
        }
        public List<OrderDish> GetOrderDishes()
        {
            try
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
            catch (OrderDishRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van alle order dishes in de repository");
                // Vang specifieke repository fouten op
                throw new OrderDishServiceException("Fout bij ophalen van alle order dishes.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van alle order dishes in de service");
                // Vang overige onverwachte fouten op
                throw new OrderDishServiceException("Onverwachte fout bij service.", ex);
            }
        }

        public List<OrderDish> GetOrderDishesForTable(int tableId)
        {
            try
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
            catch (OrderDishRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van order dishes voor tafel in de repository");
                // Vang specifieke repository fouten op
                throw new OrderDishServiceException("Fout bij ophalen van order dishes voor tafel.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van order dishes voor tafel in de service");
                // Vang overige onverwachte fouten op
                throw new OrderDishServiceException("Onverwachte fout bij service.", ex);
            }
        }
        public void AddOrderDish(OrderDishDto orderDish)
        {
            try
            {
                _orderDishRepository.AddOrderDish(orderDish);
            }
            catch (OrderDishRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het toevoegen van order dish in de repository");
                // Vang specifieke repository fouten op
                throw new OrderDishServiceException("Fout bij toevoegen van order dish.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het toevoegen van order dish in de service");
                // Vang overige onverwachte fouten op
                throw new OrderDishServiceException("Onverwachte fout bij service.", ex);
            }
        }

        public void UpdateOrderDishStatus(int tableId, string status)
        {
            try
            {

                _orderDishRepository.UpdateOrderDishStatus(tableId, status);
            }
            catch (OrderDishRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het updaten van order dish status in de repository");
                // Vang specifieke repository fouten op
                throw new OrderDishServiceException("Fout bij updaten van order dish status.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het updaten van order dish status in de service");
                // Vang overige onverwachte fouten op
                throw new OrderDishServiceException("Onverwachte fout bij service.", ex);
            }
        }
    }
}
