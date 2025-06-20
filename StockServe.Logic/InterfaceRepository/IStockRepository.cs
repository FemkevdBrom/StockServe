using Stockserve.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServe.Logic.InterfaceRepository
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllStocksAsync();
        Task UpdateOrderedStockAsync(int stockId, int orderedStock);
        Task UpdateStockQuantityAsync(int stockId, int stockQuantity);


    }
}
