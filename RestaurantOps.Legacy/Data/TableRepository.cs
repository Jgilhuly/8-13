using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
    public class TableRepository
    {
        public IEnumerable<RestaurantTable> GetAll()
        {
            const string sql = "SELECT TableId, Name, Seats, IsOccupied FROM RestaurantTables ORDER BY TableId";
            var dt = SqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                yield return new RestaurantTable
                {
                    TableId = (int)row["TableId"],
                    Name = row["Name"].ToString()!,
                    Seats = (int)row["Seats"],
                    IsOccupied = (bool)row["IsOccupied"]
                };
            }
        }

        public void UpdateOccupied(int tableId, bool occupied)
        {
            const string sql = "UPDATE RestaurantTables SET IsOccupied = @occ WHERE TableId = @id";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@occ", occupied),
                new SqlParameter("@id", tableId));
        }
    }
} 