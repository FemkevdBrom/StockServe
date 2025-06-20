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
        catch (Exception ex)
        {
            // Voeg eventueel foutafhandeling toe, zoals logging of een foutmelding in de UI
            ModelState.AddModelError(string.Empty, $"Fout bij product met ID {stockId}: {ex.Message}");
        }
    }
    return RedirectToPage(new { SearchTerm });
        }
    }
}
