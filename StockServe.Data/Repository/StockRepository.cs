using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;
using Microsoft.Data.SqlClient;
using Stockserve.Domain.Model;


namespace StockServe.Data.Repository
{
    public class StockRepository : IStockRepository
    {
        private string _connectionString = DatabaseConfig.GetConnectionString();

        public async Task<List<Stock>> GetAllStocksAsync()
        {
            var stocks = new List<Stock>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT [Id], [Name], [StockQuantity], [MinimumStock], [DesiredStock], [OrderedStock], [Supplier], [SupplierValue] FROM [dbo].[Stock]";
                try
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var stock = new Stock
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    StockQuantity = reader.GetInt32(2),
                                    MinimumStock = reader.GetInt32(3),
                                    DesiredStock = reader.GetInt32(4),
                                    OrderedStock = reader.GetInt32(5),
                                    Supplier = reader.GetString(6),
                                    SupplierValue = reader.GetInt32(7)
                                };
                                stocks.Add(stock);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new StockRepositoryException("Fout bij het ophalen van alle stock items", ex);
                }
            }
            return stocks;
        }

        public async Task UpdateOrderedStockAsync(int stockId, int orderedStock)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var query = "UPDATE Stock SET OrderedStock = @OrderedStock WHERE Id = @Id";
                try
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderedStock", orderedStock);
                        command.Parameters.AddWithValue("@Id", stockId);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new StockRepositoryException($"Fout bij het updaten van de bestelde hoeveelheid voor stock item met ID {stockId}", ex);
                }
            }
        }

        public async Task UpdateStockQuantityAsync(int stockId, int stockQuantity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var query = "UPDATE Stock SET StockQuantity = @StockQuantity WHERE Id = @Id";
                try
                {
                using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@StockQuantity", stockQuantity);
                        command.Parameters.AddWithValue("@Id", stockId);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new StockRepositoryException($"Fout bij het updaten van de voorraadhoeveelheid voor stock item met ID {stockId}", ex);
                }
                
            }
        }

    }


}
