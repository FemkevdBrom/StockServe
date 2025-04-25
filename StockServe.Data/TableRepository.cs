using Microsoft.Data.SqlClient;

namespace StockServe.Data
{
    public class TableRepository
    {
        private readonly string _connectionString = "Server=VICTUS_LE_FEMKE;Database=StockServe;Trusted_Connection=True;TrustServerCertificate=True;";


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
                    Console.WriteLine($"Error: {ex.Message}");
                    // Je kunt hier ook logging toevoegen voor meer gedetailleerde foutmeldingen
                }
            }

            return tables;
        }
    }
}

