using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Data
{
    public class OrderDishDto
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Amount { get; set; }
    }
}
