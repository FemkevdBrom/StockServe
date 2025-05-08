using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
        public string Paystatus { get; set; }

    }
}
