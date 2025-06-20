using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stockserve.Domain.Dto;
using StockServe.Logic.Service;


namespace StockServe.Pages
{
    public class BezorgLijstModel : PageModel
    {
        private readonly StockService _stockservice;

        public List<StockDto> OrderList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        [BindProperty]
        public List<int>SelectedItems { get; set; } = new();

        public BezorgLijstModel(StockService stockservice)
        {
            _stockservice = stockservice;
        }

        public async Task OnGetAsync()
        {
            OrderList = await _stockservice.GetDeliveredListAsync(SearchTerm);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedItems.Any())
            {
                await _stockservice.ProcessDeliveredItemsAsync(SelectedItems);
            }

            return RedirectToPage(new { SearchTerm }); // pagina herladen
        }
    }
}
