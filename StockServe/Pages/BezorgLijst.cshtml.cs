using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stockserve.Domain.Dto;
using StockServe.Logic.Service;
using StockServe.Logic.Exceptions;


namespace StockServe.Pages
{
    public class BezorgLijstModel : PageModel
    {
        private readonly StockService _stockservice;
        public string? ErrorMessage { get; set; }
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
            try
            {
                if (SelectedItems.Any())
                {
                    await _stockservice.ProcessDeliveredItemsAsync(SelectedItems);
                }

                return RedirectToPage(new { SearchTerm }); // pagina herladen


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
    }
}
