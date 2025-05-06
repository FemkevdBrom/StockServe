using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic;

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
            UserService userService = new UserService();
            User? user = userService.Authenticate(Email, Password);
            if (user != null)
            {
                HttpContext.Session.SetString("userRole", user.Role); // Gebruikersrol in sessie opslaan
                return RedirectToPage("/Keuzepagina");
            }
            else
                return Page();

        }
    }
}
