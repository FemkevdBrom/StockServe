using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace StockServe.Data
{
    public class OrderRepository
    {
        private string _connectionString = DatabaseConfig.GetConnectionString();


        public List<OrderDto> GetAllOrders()
        {
            var orders = new List<OrderDto>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT TOP (1000) [Id], [TableId], [Time], [Price], [Paystatus] FROM [dbo].[Order]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var order = new OrderDto
                                {
                                    Id = reader.GetInt32(0),           // [Id] is de eerste kolom in de query
                                    TableId = reader.GetInt32(1),     // [TableId] is de tweede kolom in de query
                                    Time = reader.GetDateTime(2),      // [Time] is de derde kolom in de query
                                    Price = reader.GetDecimal(3),      // [Price] is de vierde kolom in de query
                                    Paystatus = reader.GetString(4)    // [Paystatus] is de vijfde kolom in de query
                                };
                                orders.Add(order);
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
            return orders;
        }

        public void AddOrder(OrderDto order)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"INSERT INTO [Order] (TableId, Time, Price, Paystatus) 
                                   VALUES (@TableId, @Time, @Price, @Paystatus);
                                   SELECT SCOPE_IDENTITY();";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableId", order.TableId);
                        command.Parameters.AddWithValue("@Time", order.Time);
                        command.Parameters.AddWithValue("@Price", order.Price);
                        command.Parameters.AddWithValue("@Paystatus", order.Paystatus);
                        
                        order.Id = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding order: {ex.Message}");
                    throw;
                }
            }
        }

        public void UpdatePaymentStatus(int tableId, string payStatus)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE [Order] 
                        SET Paystatus = @PayStatus 
                        WHERE TableId = @TableId AND Paystatus = 'Nog niet betaald'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableId", tableId);
                        command.Parameters.AddWithValue("@PayStatus", payStatus);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating payment status: {ex.Message}");
                    throw;
                }
            }
        }

        
    }
}
