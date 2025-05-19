using Stockserve.Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic.Interface
{
    public interface IUser
    {
        UserDto GetUserEmailAndPassword(string email, string password);
    }
}
