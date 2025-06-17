using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using Microsoft.Extensions.Logging;
using StockServe.Logic.Exceptions;
using System.Data;

namespace StockServe.Data.Repository
{
    public class OrderDishRepository : IOrderDishRepository
    {
        private string _connectionString = DatabaseConfig.GetConnectionString();


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
                    throw new OrderDishRepositoryException("Fout bij het ophalen van alle order dishes", ex);
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
                        WHERE o.TableId = @TableId 
                        AND o.Paystatus = 'Nog niet betaald'
                        AND od.Status = 'Actief'
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
                    throw new OrderDishRepositoryException($"Fout bij het ophalen van order dishes voor tafel {tableId}", ex);
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
                    string query = @"INSERT INTO [OrderDish] (OrderId, DishId, Amount, Note) 
                                   VALUES (@OrderId, @DishId, @Amount, @Note)";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderId", orderDish.OrderId);
                        command.Parameters.AddWithValue("@DishId", orderDish.DishId);
                        command.Parameters.AddWithValue("@Amount", orderDish.Amount);
                        command.Parameters.AddWithValue("@Note", orderDish.Note ?? (object)DBNull.Value);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new OrderDishRepositoryException("Fout bij het ophalen van alle order dishes", ex);
                    throw;
                }
            }
        }

        public void UpdateOrderDishStatus(int tableId, string status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        UPDATE od
                        SET od.Status = @Status
                        FROM [OrderDish] od
                        JOIN [Order] o ON od.OrderId = o.Id
                        WHERE o.TableId = @TableId 
                        AND o.Paystatus = 'Nog niet betaald'
                        AND od.Status = 'Actief'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TableId", tableId);
                        command.Parameters.AddWithValue("@Status", status);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new OrderDishRepositoryException($"Fout bij het updaten van de status van order dishes voor tafel {tableId}", ex);
                    throw;
                }
            }
        }
    }
}
