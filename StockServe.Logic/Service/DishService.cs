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
    public class DishService
    {
        public readonly IDishRepository _dishRepository;
        private readonly ILogger<DishService> _logger;
        public DishService(IDishRepository dishRepository, ILogger<DishService> logger)
        {
            _dishRepository = dishRepository;
            _logger = logger;
        }
        public List<Dish> GetAllDishes()
        {
            try
            {
                List<DishDto> dishDtos = _dishRepository.GetAllDishes();
                List<Dish> dishes = new List<Dish>();
                foreach (var dishDto in dishDtos)
                {
                    dishes.Add(new Dish
                    {
                        Id = dishDto.Id,
                        Name = dishDto.Name,
                        Price = dishDto.Price,
                        Category = dishDto.Category,
                        Description = dishDto.Description

                    });
                }
                return dishes;
            }
            catch (DishRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van alle gerechten in de repository");
                throw new DishServiceException("Fout bij het ophalen van alle gerechten", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van alle gerechten in de service");
                throw new DishServiceException("Fout bij het ophalen van alle gerechten bij Service", ex);
            }
        }

        public bool DishExists(int dishId)
        {
            try
            {
                return _dishRepository.DishExists(dishId);
            }
            catch (DishRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het controleren of een gerecht bestaat in de repository");
                throw new DishServiceException("Fout bij het controleren of een gerecht bestaat", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het controleren of een gerecht bestaat in de service");
                throw new DishServiceException("Fout bij het controleren of een gerecht bestaat bij Service", ex);
            }
        }
    }
}
