using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic.Service;
using Stockserve.Domain.Model;
using Stockserve.Domain.Dto;
using StockServe.Logic.Exceptions;

namespace StockServe.Pages
{
    public class TafelDashbordModel : PageModel
    {
        private readonly TableService _tableService;

        public IList<Table>? Tables { get; set; }
        public string? ErrorMessage { get; set; }

        public TafelDashbordModel(TableService tableService)
        {
            _tableService = tableService;
        }

        public void OnGet()
        {
            try
            {
                Tables = _tableService.GetAllTables();
            }
            catch (TableServiceException ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het ophalen van de tafels:{ex.Message} ";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het ophalen van de tafels:{ex.Message} ";
            }
        }

        public IActionResult OnPostGerechtDashbord()
        {
            return RedirectToPage("/GerechtDashbord");
        }
    }
}
