using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockServe.Pages
{
    public class GerechtDashbordModel : PageModel
    {
        public int TableId { get; set; } 
        public void OnGet()
        {
            // Tafel ID ophalen uit de querystring
            if (Request.Query.ContainsKey("tableId"))
            {
                TableId = int.Parse(Request.Query["tableId"]);
            }
            else
            {
                TableId = 0; // Indien geen tafel is gevonden , stel een standaardwaarde in
            }
        }

    }
}
