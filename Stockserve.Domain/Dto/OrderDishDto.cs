using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockserve.Domain.Dto
{
    public class OrderDishDto
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; } 
        public string Note { get; set; }
    }
}
