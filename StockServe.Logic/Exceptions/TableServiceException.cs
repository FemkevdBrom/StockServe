using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic.Exceptions
{
    public class TableServiceException : Exception
    {
        public TableServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
