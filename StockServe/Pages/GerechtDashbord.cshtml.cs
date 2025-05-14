using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockServe.Logic;
using StockServe.Data;
using System;
using System.Text.Json;
using System.Linq;

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
        
        [BindProperty(SupportsGet = true)]
        public string CurrentOption { get; set; } = "Bestelling";

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

        public void OnGet(string? category = null)
        {
            try
            {
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

                // Gerechten ophalen uit de database
                DishService dishService = new DishService();
                Dishes = dishService.GetAllDishes();

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

                DishService dishService = new DishService();
                var allDishes = dishService.GetAllDishes();
                var dishToAdd = allDishes.FirstOrDefault(d => d.Id == dishId);
                
                if (dishToAdd != null)
                {
                    selectedDishes.Add(dishToAdd);
                    HttpContext.Session.SetString(SelectedDishesKey, JsonSerializer.Serialize(selectedDishes));
                }

                var currentCategory = HttpContext.Session.GetString(SelectedCategoryKey) ?? "Alle gerechten";
                return RedirectToPage(new { tableId = TableId, category = currentCategory });
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
                var orderDishService = new OrderDishService();
                TableOrderDishes = orderDishService.GetOrderDishesForTable(TableId);
            }

            // Gerechten ophalen uit de database
            DishService dishService = new DishService();
            Dishes = dishService.GetAllDishes();

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
                var orderService = new OrderService();
                var orderDishService = new OrderDishService();
                
                // Update order dish status to 'Betaald'
                orderDishService.UpdateOrderDishStatus(TableId, "Betaald");
                
                // Update order payment status
                orderService.UpdatePaymentStatus(TableId, "Betaald Cash");
                
                return RedirectToPage("/TafelDashbord", new { tableId = TableId });
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
                var orderService = new OrderService();
                var orderDishService = new OrderDishService();
                
                // Update order dish status to 'Betaald'
                orderDishService.UpdateOrderDishStatus(TableId, "Betaald");
                
                // Update order payment status
                orderService.UpdatePaymentStatus(TableId, "Betaald Pin");
                
                return RedirectToPage("/TafelDashbord", new { tableId = TableId });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het verwerken van de pin betaling: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPostBestellingToevoegen()
        {
            LoadTableId();
            try
            {
                // Debug informatie
                Console.WriteLine("=== Debug Bestelling Toevoegen ===");
                Console.WriteLine($"TableId: {TableId}");
                
                // Haal geselecteerde gerechten op uit de sessie
                var selectedDishesJson = HttpContext.Session.GetString(SelectedDishesKey);
                Console.WriteLine($"SelectedDishesJson from session: {selectedDishesJson}");
                
                if (!string.IsNullOrEmpty(selectedDishesJson))
                {
                    SelectedDishes = JsonSerializer.Deserialize<List<Dish>>(selectedDishesJson);
                    Console.WriteLine($"Aantal geselecteerde gerechten: {SelectedDishes?.Count ?? 0}");
                    
                    if (SelectedDishes != null)
                    {
                        foreach (var dish in SelectedDishes)
                        {
                            Console.WriteLine($"Gerecht: {dish.Name}, Prijs: {dish.Price}");
                        }
                    }
                }

                if (SelectedDishes != null && SelectedDishes.Any())
                {
                    var order = new Order
                    {
                        TableId = TableId,
                        Time = DateTime.Now,
                        Price = SelectedDishes.Sum(d => d.Price),
                        Paystatus = "Nog niet betaald"
                    };
                    //controleren waar of de enentueele fout bij de database ligt of bij de code kan later worden verwijderd. 
                    Console.WriteLine($"Nieuwe bestelling details:");
                    Console.WriteLine($"TableId: {order.TableId}");
                    Console.WriteLine($"Time: {order.Time}");
                    Console.WriteLine($"Price: {order.Price}");
                    Console.WriteLine($"Paystatus: {order.Paystatus}");

                    var orderService = new OrderService();
                    int orderId = orderService.AddOrder(order);

                    // Add dishes to OrderDish table
                    var orderDishRepository = new OrderDishRepository();
                    var dishService = new DishService();
                    
                    // Group dishes by ID and count occurrences
                    var groupedDishes = SelectedDishes
                        .GroupBy(d => d.Id)
                        .Select(g => new { DishId = g.Key, Amount = g.Count() });

                    foreach (var groupedDish in groupedDishes)
                    {
                        // Check if the dish exists in the database
                        if (dishService.DishExists(groupedDish.DishId))
                        {
                            var orderDish = new OrderDishDto
                            {
                                OrderId = orderId,
                                DishId = groupedDish.DishId,
                                Amount = groupedDish.Amount
                            };
                            orderDishRepository.AddOrderDish(orderDish);
                        }
                        else
                        {
                            Console.WriteLine($"Warning: Dish with ID {groupedDish.DishId} does not exist in the database");
                        }
                    }

                    // Clear the selected dishes after successful order
                    HttpContext.Session.Remove(SelectedDishesKey);
                    SelectedDishes = new List<Dish>();

                    return RedirectToPage("/TafelDashbord", new { tableId = TableId });
                }
                else
                {
                    ErrorMessage = "Er zijn geen gerechten geselecteerd om toe te voegen aan de bestelling.";
                    Console.WriteLine("Error: Geen gerechten geselecteerd");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een fout opgetreden bij het toevoegen van de bestelling: {ex.Message}";
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Page();
            }
            HttpContext.Session.Remove(SelectedDishesKey);
            HttpContext.Session.Remove(TableIdKey);
            return RedirectToPage("/TafelDashbord", new { tableId = TableId });
        }

        public IActionResult OnPostGerechtVerwijderen(int dishId)
        {
            LoadTableId();
            try
            {
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
                ErrorMessage = $"Er is een fout opgetreden bij het verwijderen van het gerecht: {ex.Message}";
                return Page();
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
