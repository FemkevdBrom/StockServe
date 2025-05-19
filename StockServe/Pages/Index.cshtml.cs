using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic.Service;
using Stockserve.Domain.Model;


namespace StockServe.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _userService;

        public IndexModel(ILogger<IndexModel> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
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
            UserService userService = _userService;
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
