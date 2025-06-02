using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockserve.UnitTest.FakeRepository
{
    public class FakeDishRepository : IDishRepository
    {
        public List<DishDto> GetAllDishes()
        {
            return new List<DishDto>
        {
            new DishDto { Id = 1, Name = "Pizza", Price = 9.99m, Category = "Main", Description = "Cheesy goodness" },
            new DishDto { Id = 2, Name = "Burger", Price = 8.49m, Category = "Main", Description = "Beefy bite" },
            new DishDto { Id = 3, Name = "Salad", Price = 5.99m, Category = "Appetizer", Description = "Fresh greens" },
            new DishDto { Id = 4, Name = "Pasta", Price = 7.99m, Category = "Main", Description = "Italian classic" }
        };
        }

        public bool DishExists(int dishId)
        {
            // Just fake the logic: true if dishId == 1 or 2 , 3 or 4. 
            return dishId == 1 || dishId == 2 || dishId == 3 || dishId ==4 ;
        }
    }
}
