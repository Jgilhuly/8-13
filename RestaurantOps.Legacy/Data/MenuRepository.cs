using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
    public class MenuRepository
    {
        public IEnumerable<MenuItem> GetAll()
        {
            const string sql = @"SELECT mi.MenuItemId, mi.CategoryId, mi.Name, mi.Description, mi.Price, mi.IsAvailable, c.Name AS CategoryName
                                FROM MenuItems mi
                                JOIN Categories c ON c.CategoryId = mi.CategoryId
                                ORDER BY c.Name, mi.Name";
            var dt = SqlHelper.ExecuteDataTable(sql);
            var list = new List<MenuItem>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(Map(row));
            }
            // Group by name to avoid duplicates if script re-ran
            foreach (var item in list.GroupBy(m => m.Name).Select(g => g.First()))
            {
                yield return item;
            }
        }

        public void Add(MenuItem item)
        {
            const string sql = @"INSERT INTO MenuItems (CategoryId, Name, Description, Price, IsAvailable)
                                 VALUES (@CategoryId, @Name, @Description, @Price, @IsAvailable)";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@CategoryId", item.CategoryId),
                new SqlParameter("@Name", item.Name),
                new SqlParameter("@Description", (object?)item.Description ?? DBNull.Value),
                new SqlParameter("@Price", item.Price),
                new SqlParameter("@IsAvailable", item.IsAvailable));
        }

        private static MenuItem Map(DataRow row) => new()
        {
            MenuItemId = (int)row["MenuItemId"],
            CategoryId = (int)row["CategoryId"],
            Name = row["Name"].ToString()!,
            Description = row["Description"].ToString(),
            Price = (decimal)row["Price"],
            IsAvailable = (bool)row["IsAvailable"],
            CategoryName = row["CategoryName"].ToString()!
        };
    }
} 