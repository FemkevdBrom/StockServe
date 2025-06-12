using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic.Exceptions
{
    public class DishRepositoryException : Exception
    {
        public DishRepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
