using Microsoft.Data.SqlClient;
using Stockserve.Domain.Dto;
using StockServe.Logic.InterfaceRepository;
using StockServe.Logic.Exceptions;


namespace StockServe.Data.Repository
{
    public class TableRepository : ITableRepository
    {
        private string _connectionString = DatabaseConfig.GetConnectionString();



        public List<TableDto> GetAllTables()
        {
            var tables = new List<TableDto>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT TOP (1000) [Id], [TableNumber] FROM [dbo].[DiningTable]";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var table = new TableDto
                                {
                                    Id = reader.GetInt32(0),           // [Id] is de eerste kolom in de query
                                    TableNumber = reader.GetInt32(1)   // [TableNumber] is de tweede kolom in de query
                                };
                                tables.Add(table);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new TableRepositoryException("Fout bij het ophalen van alle tafels", ex);
                }
            }

            return tables;
        }
    }
}

