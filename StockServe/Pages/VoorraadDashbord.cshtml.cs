using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic.Service;
using Stockserve.Domain.Dto;

namespace StockServe.Pages
{
    public class VoorraadDashbordModel : PageModel
    {
        private readonly StockService _stockService; 

        public List<StockDto> StockItems { get; set; }
        public string? SearchTerm { get; set; }


        public VoorraadDashbordModel(StockService stockService)
        {
            _stockService = stockService;
        }

        public async Task OnGetAsync(string? searchTerm)
        {
            StockItems = await _stockService.GetAllStocksAsync();
            SearchTerm = searchTerm;
            var allStocks = await _stockService.GetAllStocksAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                StockItems = allStocks
                    .Where(s => s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                StockItems = allStocks;
            }
        }


    }
                   
}
