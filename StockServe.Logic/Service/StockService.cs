using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Model;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;

namespace StockServe.Logic.Service
{
    public class StockService
    {
        private readonly IStockRepository _stockRepository;
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }
        public async Task<List<StockDto>> GetAllStocksAsync()
        {
            var stockList = await _stockRepository.GetAllStocksAsync();
            return stockList.Select(s => new StockDto
            {
                Id = s.Id,
                Name = s.Name,
                StockQuantity = s.StockQuantity,
                MinimumStock = s.MinimumStock,
                DesiredStock = s.DesiredStock,
                OrderedStock = s.OrderedStock,
                Supplier = s.Supplier,
                SupplierValue = s.SupplierValue
            }).ToList();
        }
        public async Task<List<StockDto>> GetOrderListAsync(string? searchTerm = null)
        {
            var allStock = await _stockRepository.GetAllStocksAsync();

            var filtered = allStock
                .Where(s => s.StockQuantity <= s.MinimumStock) 
                .Where(s => string.IsNullOrEmpty(searchTerm) || s.Name.ToLower().Contains(searchTerm.ToLower()))
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    StockQuantity = s.StockQuantity,
                    MinimumStock = s.MinimumStock,
                    DesiredStock = s.DesiredStock,
                    OrderedStock = s.OrderedStock,
                    Supplier = s.Supplier,
                    SupplierValue = s.SupplierValue
                })
                .ToList();

            return filtered;
        }
        public async Task<List<StockDto>> GetDeliveredListAsync(string? searchTerm = null)
        {
            var allStock = await _stockRepository.GetAllStocksAsync();

            var filtered = allStock
                .Where(s => s.StockQuantity <= s.MinimumStock)
                .Where(s => s.OrderedStock > 0) // Alleen producten met een bestelde hoeveelheid > 0
                .Where(s => string.IsNullOrEmpty(searchTerm) || s.Name.ToLower().Contains(searchTerm.ToLower()))
                .Select(s => new StockDto
                
                {
                    Id = s.Id,
                    Name = s.Name,
                    StockQuantity = s.StockQuantity,
                    MinimumStock = s.MinimumStock,
                    DesiredStock = s.DesiredStock,
                    OrderedStock = s.OrderedStock,
                    Supplier = s.Supplier,
                    SupplierValue = s.SupplierValue
                })
                .ToList();

            return filtered;
        }

        public async Task UpdateBestellingAsync(int stockId, int orderedQuantity)
        {
            var stock = (await _stockRepository.GetAllStocksAsync()).FirstOrDefault(s => s.Id == stockId);
            if (stock == null)
                throw new StockServiceException("Product niet gevonden.", null);

            if (orderedQuantity % stock.SupplierValue != 0)
                throw new StockServiceException($"Bestelhoeveelheid moet een veelvoud zijn van {stock.SupplierValue}.", null);

            // Update OrderedStock in het model
            stock.OrderedStock = orderedQuantity;

            // Pas de update in de database toe via repository
            await _stockRepository.UpdateOrderedStockAsync(stock.Id, orderedQuantity);
        }

        public async Task ProcessDeliveredItemsAsync(List<int> selectedItemIds)
        {
            var allStock = await _stockRepository.GetAllStocksAsync();
            var selectedStocks = allStock.Where(s => selectedItemIds.Contains(s.Id)).ToList();

            foreach (var stock in selectedStocks)
            {
                stock.StockQuantity += stock.OrderedStock;
                stock.OrderedStock = 0;

                await _stockRepository.UpdateStockQuantityAsync(stock.Id, stock.StockQuantity);
                await _stockRepository.UpdateOrderedStockAsync(stock.Id, 0);
            }
        }

        public async Task UpdateStockQuantityAsync(int stockId, int nieuweVoorraad)
        {
            var stock = (await _stockRepository.GetAllStocksAsync()).FirstOrDefault(s => s.Id == stockId);
            if (stock == null)
                throw new StockServiceException("Product niet gevonden", null);

            stock.StockQuantity = nieuweVoorraad;

            await _stockRepository.UpdateStockQuantityAsync(stock.Id, nieuweVoorraad);
        }


    }
}
