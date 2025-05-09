using System;

namespace StockServe.Data
{
    public static class DatabaseConfig
    {
        private static readonly string _connectionString = "Server=VICTUS_LE_FEMKE;Database=StockServe;Trusted_Connection=True;TrustServerCertificate=True;";

        public static string GetConnectionString()
        {
            return _connectionString;
        }
    }
} 