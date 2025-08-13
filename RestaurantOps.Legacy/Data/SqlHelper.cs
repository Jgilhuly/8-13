using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace RestaurantOps.Legacy.Data
{
    // Simple legacy-style helper to execute SQL commands synchronously.
    public static class SqlHelper
    {
        private static string? _connectionString;

        public static void Initialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        private static SqlConnection GetConnection()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new InvalidOperationException("SqlHelper not initialized. Call Initialize first.");
            return new SqlConnection(_connectionString);
        }

        public static DataTable ExecuteDataTable(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            using var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public static object? ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters.Length > 0) cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteScalar();
        }
    }
} 