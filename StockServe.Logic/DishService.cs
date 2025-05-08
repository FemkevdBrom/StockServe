using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockServe.Data;

namespace StockServe.Logic
{
    public class DishService
    {
        public List<Dish> GetAllDishes()
        {
            List<DishDto> dishDtos = new DishRepository().GetAllDishes();
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

        public bool DishExists(int dishId)
        {
            return new DishRepository().DishExists(dishId);
        }
    }
}
