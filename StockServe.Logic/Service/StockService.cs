using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Model;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;
using Microsoft.Extensions.Logging;
namespace StockServe.Logic.Service
{
    public class StockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly ILogger<StockService> _logger;
        public StockService(IStockRepository stockRepository, ILogger<StockService> logger)
        {
            _stockRepository = stockRepository;
            _logger = logger;
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
            try
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
            catch (StockRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van de bestellijst in de repository");
                throw new StockServiceException("Fout bij het ophalen van de bestellijst.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onverwachte fout bij het ophalen van de bestellijst in de service");
                throw new StockServiceException("Fout bij het ophalen van de bestellijst.", ex);
            }
        }
        public async Task<List<StockDto>> GetDeliveredListAsync(string? searchTerm = null)
        {
            try
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
            catch (StockRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het ophalen van de geleverde lijst in de repository");
                throw new StockServiceException("Fout bij het ophalen van de geleverde lijst.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onverwachte fout bij het ophalen van de geleverde lijst in de service");
                throw new StockServiceException("Fout bij het ophalen van de geleverde lijst.", ex);
            }

        }

        public async Task UpdateBestellingAsync(int stockId, int orderedQuantity)
        {
            try
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
            catch (StockRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het updaten van de bestelling in de repository");
                throw new StockServiceException("Fout bij het updaten van de bestelling.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onverwachte fout bij het updaten van de bestelling in de service");
                throw new StockServiceException("Fout bij het updaten van de bestelling.", ex);
            }
        }

        public async Task ProcessDeliveredItemsAsync(List<int> selectedItemIds)
        {
            try
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
            catch (StockRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het verwerken van geleverde items in de repository");
                throw new StockServiceException("Fout bij het verwerken van geleverde items.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onverwachte fout bij het verwerken van geleverde items in de service");
                throw new StockServiceException("Fout bij het verwerken van geleverde items.", ex);
            }
        }

        public async Task UpdateStockQuantityAsync(int stockId, int nieuweVoorraad)
        {
            try
            {
            var stock = (await _stockRepository.GetAllStocksAsync()).FirstOrDefault(s => s.Id == stockId);
                        if (stock == null)
                            throw new StockServiceException("Product niet gevonden", null);

                        stock.StockQuantity = nieuweVoorraad;

                        await _stockRepository.UpdateStockQuantityAsync(stock.Id, nieuweVoorraad);
            }
            catch(StockRepositoryException ex)
            {
                _logger.LogError(ex, "Fout bij het updaten van de voorraad in de repository");
                throw new StockServiceException("Fout bij het updaten van de voorraad.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Onverwachte fout bij het updaten van de voorraad in de service");
                throw new StockServiceException("Fout bij het updaten van de voorraad.", ex);
            }
        }
    }
}
