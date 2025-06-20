using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stockserve.Domain.Model
{
    public class Stock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStock { get; set; }
        public int DesiredStock { get; set; }
        public int OrderedStock { get; set; }
        public string Supplier { get; set; }
        public int SupplierValue { get; set; }
    }
}
