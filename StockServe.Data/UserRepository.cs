using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace StockServe.Data
{
    public class UserRepository
    {
        private string _connectionString = "Server=VICTUS_LE_FEMKE;Database=StockServe;Trusted_Connection=True;TrustServerCertificate=True;";

        public UserDto GetUserEmailAndPassword (string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT [Id], [Name], [Email], [Password], [EmployeeCode], [Role] FROM [User] WHERE [Email] = @Email AND [Password] = @Password";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserDto
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                EmployeeCode = reader.GetInt32(4),
                                Role = reader.GetString(5)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during login: " + ex.Message);
                }
            }

            return null;
        }
    }
}
