using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stockserve.Domain.Dto;
using StockServe.Logic.Service;
using StockServe.Logic.Exceptions;

namespace StockServe.Pages
{
    public class BestellijstModel : PageModel
    {
        private readonly StockService _stockservice;

        public List<StockDto> OrderList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }

        [BindProperty]
        [FromForm(Name = "OrderedQuantities")]
        public Dictionary<int, int> OrderedQuantities { get; set; } = new();

        public BestellijstModel(StockService stockservice)
        {
            _stockservice = stockservice;
        }
        
        public async Task OnGetAsync()
        {
            OrderList = await _stockservice.GetOrderListAsync(SearchTerm);
        }


        public async Task<IActionResult> OnPostAsync()
        {
            foreach (var entry in OrderedQuantities)
    {
        int stockId = entry.Key;
        int orderedQuantity = entry.Value;

        try
        {
            await _stockservice.UpdateBestellingAsync(stockId, orderedQuantity);
        }
        catch (StockServiceException ex)
        {
                    ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                    return Page();
                }
                catch (Exception ex)
        {
                    ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                    return Page();
                }
        }
        return RedirectToPage(new { SearchTerm });
        }
    }
}
