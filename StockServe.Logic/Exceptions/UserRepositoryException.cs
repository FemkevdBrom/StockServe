using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic.Exceptions
{
    public class UserRepositoryException : Exception
    {
        public UserRepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
