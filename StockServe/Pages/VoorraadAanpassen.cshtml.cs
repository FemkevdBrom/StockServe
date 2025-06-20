using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stockserve.Domain.Dto;
using StockServe.Logic.Service;


namespace StockServe.Pages
{
    public class VoorraadAanpassenModel : PageModel
    {
        private readonly StockService _stockService;

        public List<StockDto> StockItems { get; set; }
        public string? SearchTerm { get; set; }

        public VoorraadAanpassenModel(StockService stockService)
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

        public async Task<IActionResult> OnPostAsync(List<StockDto> stockItems)
        {
            foreach (var item in stockItems)
            {
                if (item.UsedQuantity < 0 || item.UsedQuantity > item.StockQuantity)
                {
                    ModelState.AddModelError("", $"Ongeldige waarde voor {item.Name}");
                    continue;
                }

                var nieuweVoorraad = item.StockQuantity - item.UsedQuantity;

                await _stockService.UpdateStockQuantityAsync(item.Id, nieuweVoorraad);
            }

            return RedirectToPage("VoorraadAanpassen", new { SearchTerm });
        }
    }
}
