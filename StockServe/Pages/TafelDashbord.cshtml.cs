using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic;

namespace StockServe.Pages
{
    public class TafelDashbordModel : PageModel
    {
        private readonly TableService _tableService;
        public IList<Table>? Tables { get; set; }

        public TafelDashbordModel(TableService tableService)
        {
            _tableService = tableService;
        }

        public void OnGet()
        {
            Tables = _tableService.GetAllTables();
        }

        public IActionResult OnPostGerechtDashbord()
        {
            return RedirectToPage("/GerechtDashbord");
        }
    }
}
