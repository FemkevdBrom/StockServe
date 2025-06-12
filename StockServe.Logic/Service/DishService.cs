using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Model;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;

namespace StockServe.Logic.Service
{   
    public class DishService
    {
        public readonly IDishRepository _dishRepository;
        public DishService(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
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
                throw new Exception("Fout bij het ophalen van alle gerechten", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij het ophalen van alle gerechten bij Service", ex);
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
                throw new Exception("Fout bij het controleren of een gerecht bestaat", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Fout bij het controleren of een gerecht bestaat bij Service", ex);
            }
        }
    }
}
