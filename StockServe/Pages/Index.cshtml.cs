using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic.Service;
using Stockserve.Domain.Model;
using StockServe.Logic.Exceptions;


namespace StockServe.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly UserService _userService;
        public string ErrorMessage { get; set; }

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
            try
            {
                UserService userService = _userService;
                User? user = userService.Authenticate(Email, Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("userRole", user.Role);
                    return RedirectToPage("/Keuzepagina");
                }
                else
                {
                    ErrorMessage = "Ongeldige gebruikersnaam of wachtwoord.";
                    return Page();
                }
            }
            catch (UserServiceException ex)
            {
                _logger.LogError(ex, "Fout bij inloggen: {Email}", Email);
                ErrorMessage = "Er is een fout opgetreden tijdens het inloggen. Probeer het later opnieuw.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden tijdens het inloggen. Probeer het later opnieuw.: {ex.Message} ";
                return Page();
            }
        }
    }
}
