using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockServe.Pages
{
    public class TafelDashbordModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult onPostGerechtDashbord()
        {
            return RedirectToPage("/GerechtDashbord");
        }
    }
}
