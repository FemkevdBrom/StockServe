using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;


namespace StockServe.Pages
{
    public class KeuzepaginaModel : PageModel
    {
        public string userRole { get; set; } = "";
        public void OnGet()
        {
            userRole = HttpContext.Session.GetString("userRole") ?? "";

            Console.WriteLine($"User Role: {userRole}");
        }

        public IActionResult OnPostVoorraaddashbord()
        {
            var role = HttpContext.Session.GetString("userRole");
            if (role == "Baas" || role == "Manager")
            {
                return RedirectToPage("/VoorraadDashbord");
            }

            // Als de rol niet correct is, stuur dan terug naar de Keuzepagina met een foutmelding
            return RedirectToPage("/Keuzepagina");
        }

        public IActionResult OnPostTafeldashbord()
        {
            var role = HttpContext.Session.GetString("userRole");
                return RedirectToPage("/TafelDashbord");
            
        }
    }
}
