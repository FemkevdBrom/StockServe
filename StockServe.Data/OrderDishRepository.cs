using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using StockServe.Data;
namespace StockServe.Data
{
    public class OrderDishRepository
    {
        private string _connectionString = "Server=VICTUS_LE_FEMKE;Database=StockServe;Trusted_Connection=True;TrustServerCertificate=True;";

        public List<OrderDishDto> GetOrderDishes()
        {
            var orderDishes = new List<OrderDishDto>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT TOP (1000) [OrderId], [DishId], [Amount] FROM [dbo].[OrderDish]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var orderDish = new OrderDishDto
                                {
                                    OrderId = reader.GetInt32(0),           // [OrderId] is de eerste kolom in de query
                                    DishId = reader.GetInt32(1),             // [DishId] is de tweede kolom in de query
                                    Amount = reader.GetInt32(2)               // [Amount] is de derde kolom in de query
                                };
                                orderDishes.Add(orderDish);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    // Je kunt hier ook logging toevoegen voor meer gedetailleerde foutmeldingen
                }
            }
            return orderDishes;

        }

        public List<OrderDishDto> GetOrderDishesForTable(int tableId)
        {
            var orderDishes = new List<OrderDishDto>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        SELECT od.DishId, SUM(od.Amount) as TotalAmount
                        FROM [OrderDish] od
                        JOIN [Order] o ON od.OrderId = o.Id
                        WHERE o.TableId = @TableId AND o.Paystatus = 'Nog niet betaald'
                        GROUP BY od.DishId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableId", tableId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var orderDish = new OrderDishDto
                                {
                                    DishId = reader.GetInt32(0),
                                    Amount = reader.GetInt32(1)
                                };
                                orderDishes.Add(orderDish);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting order dishes for table: {ex.Message}");
                }
            }
            return orderDishes;
        }

        public void AddOrderDish(OrderDishDto orderDish)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"INSERT INTO [OrderDish] (OrderId, DishId, Amount) 
                                   VALUES (@OrderId, @DishId, @Amount)";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderDish.OrderId);
                        command.Parameters.AddWithValue("@DishId", orderDish.DishId);
                        command.Parameters.AddWithValue("@Amount", orderDish.Amount);
                        
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding order dish: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
