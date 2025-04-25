using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic;

namespace StockServe.Pages
{
    public class TafelDashbordModel : PageModel
    {
        public IList<Table>? Tables { get; set; }
        public void OnGet()
        {
            TableService tableService = new TableService();
            Tables = tableService.GetAllTables();

        }
        public IActionResult OnPostGerechtDashbord()
        {
            return RedirectToPage("/GerechtDashbord");
        }
    }
}
