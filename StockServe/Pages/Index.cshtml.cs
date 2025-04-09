using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockServe.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
            
        public IActionResult OnPost()
        {
            //Voeg logica toe om de gebruiker in te loggen
            //Bijvoorbeeld: controleer of de gebruikersnaam en het wachtwoord correct zijn
            return RedirectToPage("/Keuzepagina");

        }
    }
}
