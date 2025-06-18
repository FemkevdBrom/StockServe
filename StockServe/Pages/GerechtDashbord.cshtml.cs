using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stockserve.Domain.Dto;
using Stockserve.Domain.Model;
using StockServe.Data;
using StockServe.Logic;
using StockServe.Logic.Service;
using StockServe.Logic.Exceptions;
using System;
using System.Linq;
using System.Text.Json;


namespace StockServe.Pages
{
    public class GerechtDashbordModel : PageModel
    {
        public int TableId { get; set; } 
        public IList<Dish>? Dishes { get; set; }
        public IList<Dish>? SelectedDishes { get; set; }
        public IList<OrderDish>? TableOrderDishes { get; set; }
        public string? SelectedCategory { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public string? ErrorMessage { get; set; }

        private const string SelectedDishesKey = "SelectedDishes";
        private const string TableIdKey = "CurrentTableId";
        private const string SelectedCategoryKey = "SelectedCategory";
        private const string DishNotesKey = "DishNotes";
        
        [BindProperty(SupportsGet = true)]
        public string CurrentOption { get; set; } = "Bestelling";

        private readonly DishService _dishService; 
        private readonly OrderDishService _orderDishService;
        private readonly OrderService _orderService;
        public GerechtDashbordModel(DishService dishService, OrderDishService orderDishService, OrderService orderService)
        {
            _dishService = dishService;
            _orderDishService = orderDishService;
            _orderService = orderService;
        }

        private void LoadTableId()
        {
            if (Request.Query.ContainsKey("tableId"))
            {
                TableId = int.Parse(Request.Query["tableId"]);
                HttpContext.Session.SetInt32(TableIdKey, TableId);
            }
            else
            {
                TableId = HttpContext.Session.GetInt32(TableIdKey) ?? 0;
            }
        }

        public void OnGet(string? category = null, string? selectedDishes = null)
        {
            try
            {
                LoadTableId();

                // Haal geselecteerde gerechten op uit de query parameters of sessie
                if (!string.IsNullOrEmpty(selectedDishes))
                {
                    SelectedDishes = JsonSerializer.Deserialize<List<Dish>>(selectedDishes);
                    HttpContext.Session.SetString(SelectedDishesKey, selectedDishes);
                }
                else
                {
                    var selectedDishesJson = HttpContext.Session.GetString(SelectedDishesKey);
                    if (!string.IsNullOrEmpty(selectedDishesJson))
                    {
                        SelectedDishes = JsonSerializer.Deserialize<List<Dish>>(selectedDishesJson);
                    }
                    else
                    {
                        SelectedDishes = new List<Dish>();
                    }
                }

                // Gerechten ophalen uit de database
                
                Dishes = _dishService.GetAllDishes();

                if (Dishes == null || !Dishes.Any())
                {
                    ErrorMessage = "Geen gerechten gevonden in de database.";
                    return;
                }

                // Voeg 'Alle gerechten' als eerste item in de lijst
                Categories = Dishes.Select(d => d.Category).Distinct().ToList();
                Categories.Insert(0, "Alle gerechten");

                // Haal de geselecteerde categorie op uit de sessie of gebruik de parameter
                if (!string.IsNullOrEmpty(category))
                {
                    SelectedCategory = category;
                    HttpContext.Session.SetString(SelectedCategoryKey, category);
                }
                else
                {
                    SelectedCategory = HttpContext.Session.GetString(SelectedCategoryKey) ?? "Alle gerechten";
                }

                // Filter op categorie als er een is geselecteerd, behalve als 'Alle gerechten' geselecteerd is
                if (SelectedCategory != "Alle gerechten")
                {
                    Dishes = Dishes.Where(d => d.Category == SelectedCategory).ToList();
                }
            }
            catch (DishServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden: {ex.Message}";
                Console.WriteLine($"Error in OnGet: {ex}");
            }
        }

        public IActionResult OnPostAddToOrder(int dishId)
        {
            LoadTableId();
            try
            {
                var selectedDishesJson = HttpContext.Session.GetString(SelectedDishesKey);
                var selectedDishes = !string.IsNullOrEmpty(selectedDishesJson) 
                    ? JsonSerializer.Deserialize<List<Dish>>(selectedDishesJson) 
                    : new List<Dish>();

                
                var allDishes = _dishService.GetAllDishes();
                var dishToAdd = allDishes.FirstOrDefault(d => d.Id == dishId);
                
                if (dishToAdd != null)
                {
                    selectedDishes.Add(dishToAdd);
                    HttpContext.Session.SetString(SelectedDishesKey, JsonSerializer.Serialize(selectedDishes));
                }

                var currentCategory = HttpContext.Session.GetString(SelectedCategoryKey) ?? "Alle gerechten";
                return RedirectToPage(new { tableId = TableId, category = currentCategory });
            }
            catch (DishServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het toevoegen van het gerecht: {ex.Message}";
                var currentCategory = HttpContext.Session.GetString(SelectedCategoryKey) ?? "Alle gerechten";
                return RedirectToPage(new { tableId = TableId, category = currentCategory });
            }
        }

        public IActionResult OnPostSetOptionType(string optionType)
        {
            try
            {
                CurrentOption = optionType;
                LoadTableId();

                // Haal geselecteerde gerechten op uit de sessie
                var selectedDishesJson = HttpContext.Session.GetString(SelectedDishesKey);
                if (!string.IsNullOrEmpty(selectedDishesJson))
                {
                    SelectedDishes = JsonSerializer.Deserialize<List<Dish>>(selectedDishesJson);
                }
                else
                {
                    SelectedDishes = new List<Dish>();
                }

                // Als Rekening of Betalen is geselecteerd, haal de bestelde gerechten op
                if (optionType == "Rekening" || optionType == "Betalen")
                {
                    var orderDishService = _orderDishService;
                    TableOrderDishes = orderDishService.GetOrderDishesForTable(TableId);
                }

                // Gerechten ophalen uit de database

                Dishes = _dishService.GetAllDishes();

                if (Dishes == null || !Dishes.Any())
                {
                    ErrorMessage = "Geen gerechten gevonden in de database.";
                    return Page();
                }

                // Voeg 'Alle gerechten' als eerste item in de lijst
                Categories = Dishes.Select(d => d.Category).Distinct().ToList();
                Categories.Insert(0, "Alle gerechten");

                // Haal de geselecteerde categorie op uit de sessie
                SelectedCategory = HttpContext.Session.GetString(SelectedCategoryKey) ?? "Alle gerechten";

                // Filter op categorie als er een is geselecteerd, behalve als 'Alle gerechten' geselecteerd is
                if (SelectedCategory != "Alle gerechten")
                {
                    Dishes = Dishes.Where(d => d.Category == SelectedCategory).ToList();
                }

                return Page();
            }
            catch (DishServiceException ex)
            {
                    ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                    Console.WriteLine(ErrorMessage);
                    Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (OrderDishServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het instellen van de optie: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPostNaarTafelDashbord()
        {
            LoadTableId();
            HttpContext.Session.Remove(SelectedDishesKey);
            HttpContext.Session.Remove(TableIdKey);
            return RedirectToPage("/TafelDashbord", new { tableId = TableId });
        }

        public IActionResult OnPostCash() 
        {
            LoadTableId();
            try
            {
                var orderService = _orderService;
                var orderDishService = _orderDishService;
                
                // Update order dish status to 'Betaald'
                orderDishService.UpdateOrderDishStatus(TableId, "Betaald");
                
                // Update order payment status
                orderService.UpdatePaymentStatus(TableId, "Betaald Cash");
                
                return RedirectToPage("/TafelDashbord", new { tableId = TableId });
            }
            catch (OrderDishServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                return Page();
            }
            catch (OrderServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het verwerken van de cash betaling: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPostPin() 
        {
            LoadTableId();
            try
            {
                var orderService = _orderService;
                var orderDishService = _orderDishService;

                // Update order dish status to 'Betaald'
                orderDishService.UpdateOrderDishStatus(TableId, "Betaald");

                // Update order payment status
                orderService.UpdatePaymentStatus(TableId, "Betaald Pin");

                return RedirectToPage("/TafelDashbord", new { tableId = TableId });
            }
            catch (OrderDishServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (OrderServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het verwerken van de pin betaling: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPostOpmerkingToevoegen(int dishId, string note)
        {
            try
            {
                LoadTableId();
                
                // Get existing notes from session
                var notesJson = HttpContext.Session.GetString(DishNotesKey);
                var notes = !string.IsNullOrEmpty(notesJson) 
                    ? JsonSerializer.Deserialize<Dictionary<int, string>>(notesJson) 
                    : new Dictionary<int, string>();

                // Add or update note for the dish
                notes[dishId] = note;
                
                // Save back to session
                HttpContext.Session.SetString(DishNotesKey, JsonSerializer.Serialize(notes));

                // Redirect back to the same page with current option
                return RedirectToPage(new { tableId = TableId });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het toevoegen van de opmerking: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPostBestellingToevoegen()
        {
            try
            {
                LoadTableId();
                var selectedDishesJson = HttpContext.Session.GetString(SelectedDishesKey);
                var notesJson = HttpContext.Session.GetString(DishNotesKey);
                var notes = !string.IsNullOrEmpty(notesJson) 
                    ? JsonSerializer.Deserialize<Dictionary<int, string>>(notesJson) 
                    : new Dictionary<int, string>();

                if (string.IsNullOrEmpty(selectedDishesJson))
                {
                    ErrorMessage = "Geen gerechten geselecteerd om toe te voegen.";
                    return Page();
                }

                var selectedDishes = JsonSerializer.Deserialize<List<Dish>>(selectedDishesJson);
                if (selectedDishes == null || !selectedDishes.Any())
                {
                    ErrorMessage = "Geen gerechten geselecteerd om toe te voegen.";
                    return Page();
                }

                // Create new order
                var order = new Order
                {
                    TableId = TableId,
                    Time = DateTime.Now,
                    Price = selectedDishes.Sum(d => d.Price),
                    Paystatus = "Nog niet betaald"
                };

                var orderService = _orderService;
                int orderId = orderService.AddOrder(order);

                // Group dishes by ID and count occurrences
                var groupedDishes = selectedDishes
                    .GroupBy(d => d.Id)
                    .Select(g => new { DishId = g.Key, Amount = g.Count() });

                foreach (var groupedDish in groupedDishes)
                {
                    var orderDish = new OrderDishDto
                    {
                        OrderId = orderId,
                        DishId = groupedDish.DishId,
                        Amount = groupedDish.Amount,
                        Status = "Actief",
                        Note = notes.ContainsKey(groupedDish.DishId) ? notes[groupedDish.DishId] : null
                    };
                    _orderDishService.AddOrderDish(orderDish);
                }

                // Clear session data
                HttpContext.Session.Remove(SelectedDishesKey);
                HttpContext.Session.Remove(DishNotesKey);

                return RedirectToPage("/TafelDashbord", new { tableId = TableId });
            }
            catch (OrderServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (OrderDishServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (DishServiceException ex)
            {
                ErrorMessage = ex.Message; // Gebruik enkel de tekst uit de service
                Console.WriteLine(ErrorMessage);
                Console.WriteLine(ex.StackTrace);
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het toevoegen van de bestelling: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPostGerechtVerwijderen(int dishId)
        {
            LoadTableId();
            try
            {
                // Verwijder het gerecht uit de geselecteerde gerechten
                var selectedDishesJson = HttpContext.Session.GetString(SelectedDishesKey);
                if (!string.IsNullOrEmpty(selectedDishesJson))
                {
                    SelectedDishes = JsonSerializer.Deserialize<List<Dish>>(selectedDishesJson);
                    if (SelectedDishes != null)
                    {
                        // Converteer naar List en vind de eerste index van het gerecht met de gegeven ID
                        var dishesList = SelectedDishes.ToList();
                        var indexToRemove = dishesList.FindIndex(d => d.Id == dishId);
                        if (indexToRemove != -1)
                        {
                            // Verwijder alleen het eerste gevonden gerecht
                            dishesList.RemoveAt(indexToRemove);
                            SelectedDishes = dishesList;
                            HttpContext.Session.SetString(SelectedDishesKey, JsonSerializer.Serialize(SelectedDishes));
                        }
                    }
                }

                // Terug naar de normale weergave met de geselecteerde gerechten
                var currentCategory = HttpContext.Session.GetString(SelectedCategoryKey) ?? "Alle gerechten";
                return RedirectToPage(new { 
                    tableId = TableId, 
                    category = currentCategory, 
                    optionType = "Bestelling",
                    selectedDishes = JsonSerializer.Serialize(SelectedDishes)
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het verwijderen van het gerecht: {ex.Message}";
                var currentCategory = HttpContext.Session.GetString(SelectedCategoryKey) ?? "Alle gerechten";
                return RedirectToPage(new { 
                    tableId = TableId, 
                    category = currentCategory, 
                    optionType = "Bestelling",
                    selectedDishes = JsonSerializer.Serialize(SelectedDishes)
                });
            }
        }

        public IActionResult OnPostOpmerkingToevoegenGerecht(int dishId)
        {
            LoadTableId();
            try {
                var selectedDishesJson = HttpContext.Session.GetString(SelectedDishesKey);
                if (!string.IsNullOrEmpty(selectedDishesJson))
                {
                    SelectedDishes = JsonSerializer.Deserialize<List<Dish>>(selectedDishesJson);
                    if (SelectedDishes != null)
                    {
                        SelectedDishes = SelectedDishes.Where(d => d.Id != dishId).ToList();
                        HttpContext.Session.SetString(SelectedDishesKey, JsonSerializer.Serialize(SelectedDishes));
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het Toevoegen van een opmerking aan het gerecht: {ex.Message}";
                return Page();
            }
        }

        //nodig om de buttens goed te kunnen laden.
        public IActionResult OnPostRekening()
        {
            LoadTableId();
            return Page();
        }

        public IActionResult OnPostBetalen()
        {
            LoadTableId();
            return Page();
        }

        public IActionResult OnPostTerug()
        {
            LoadTableId();
            return Page();
        }
    }
}
