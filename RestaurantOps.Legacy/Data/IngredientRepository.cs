using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
    public class IngredientRepository
    {
        public IEnumerable<Ingredient> GetAll()
        {
            const string sql = "SELECT IngredientId, Name, Unit, QuantityOnHand, ReorderThreshold FROM Ingredients ORDER BY Name";
            var dt = SqlHelper.ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                yield return Map(row);
            }
        }

        public Ingredient? GetById(int id)
        {
            const string sql = "SELECT IngredientId, Name, Unit, QuantityOnHand, ReorderThreshold FROM Ingredients WHERE IngredientId = @id";
            var dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@id", id));
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public void Add(Ingredient ing)
        {
            const string sql = @"INSERT INTO Ingredients (Name, Unit, QuantityOnHand, ReorderThreshold)
                                 VALUES (@Name, @Unit, @Qty, @Threshold)";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Name", ing.Name),
                new SqlParameter("@Unit", ing.Unit),
                new SqlParameter("@Qty", ing.QuantityOnHand),
                new SqlParameter("@Threshold", ing.ReorderThreshold));
        }

        public void Update(Ingredient ing)
        {
            const string sql = @"UPDATE Ingredients SET Name=@Name, Unit=@Unit, QuantityOnHand=@Qty, ReorderThreshold=@Threshold
                                 WHERE IngredientId=@Id";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Name", ing.Name),
                new SqlParameter("@Unit", ing.Unit),
                new SqlParameter("@Qty", ing.QuantityOnHand),
                new SqlParameter("@Threshold", ing.ReorderThreshold),
                new SqlParameter("@Id", ing.IngredientId));
        }

        private static Ingredient Map(DataRow row) => new()
        {
            IngredientId = (int)row["IngredientId"],
            Name = row["Name"].ToString()!,
            Unit = row["Unit"].ToString()!,
            QuantityOnHand = (decimal)row["QuantityOnHand"],
            ReorderThreshold = (decimal)row["ReorderThreshold"]
        };
    }
} 