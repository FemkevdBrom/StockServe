using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockServe.Pages
{
    public class KeuzepaginaModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult onPostVoorraaddashboard()
        {
            return RedirectToPage("/Voorraaddashbord");
        }

        public IActionResult onPostTafeldashbord()
        {
            return RedirectToPage("/Tafeldashbord");
        }
    }
}
