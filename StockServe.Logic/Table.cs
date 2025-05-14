using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic
{
    public class Table
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public bool HasActiveOrders { get; set; }
    }
}
