using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockServe.Pages
{
    public class KeuzepaginaModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostVoorraaddashboard()
        {
            return RedirectToPage("/Voorraaddashbord");
        }

        public IActionResult OnPostTafeldashbord()
        {
            return RedirectToPage("/TafelDashbord");
        }
    }
}
