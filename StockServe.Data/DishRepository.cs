using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace StockServe.Data
{
    public class DishRepository
    {
        private string _connectionString = DatabaseConfig.GetConnectionString();

        public List<DishDto> GetAllDishes()
        {
            var dishes = new List<DishDto>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT TOP (1000) [Id], [Name], [Price], [Category], [Description] FROM [dbo].[Dish]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dish = new DishDto
                                {
                                    Id = reader.GetInt32(0),           // [Id] is de eerste kolom in de query
                                    Name = reader.GetString(1),        // [Name] is de tweede kolom in de query
                                    Price = reader.GetDecimal(2),      // [Price] is de derde kolom in de query
                                    Category = reader.GetString(3),    // [Category] is de vierde kolom in de query
                                    Description = reader.GetString(4)  // [Description] is de vijfde kolom in de query
                                };
                                dishes.Add(dish);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return dishes;
        }

        public bool DishExists(int dishId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(1) FROM [Dish] WHERE Id = @DishId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DishId", dishId);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking if dish exists: {ex.Message}");
                    return false;
                }
            }
        }
    }
} 