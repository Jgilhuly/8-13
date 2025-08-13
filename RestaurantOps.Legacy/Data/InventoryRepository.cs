using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
    public class InventoryRepository
    {
        public void AdjustStock(int ingredientId, decimal quantityChange, string? notes)
        {
            const string insertSql = @"INSERT INTO InventoryTx (IngredientId, QuantityChange, Notes) 
                                         VALUES (@id, @chg, @notes)";
            const string updateSql = "UPDATE Ingredients SET QuantityOnHand = QuantityOnHand + @chg WHERE IngredientId = @id";

            var noteParam = new SqlParameter("@notes", (object?)notes ?? DBNull.Value);

            // Record transaction
            SqlHelper.ExecuteNonQuery(insertSql,
                new SqlParameter("@id", ingredientId),
                new SqlParameter("@chg", quantityChange),
                noteParam);

            // Update running balance
            SqlHelper.ExecuteNonQuery(updateSql,
                new SqlParameter("@chg", quantityChange),
                new SqlParameter("@id", ingredientId));
        }

        public IEnumerable<InventoryTx> GetByIngredient(int ingredientId)
        {
            const string sql = @"SELECT tx.TxId, tx.IngredientId, tx.TxDate, tx.QuantityChange, tx.Notes,
                                        i.Name AS IngredientName
                                   FROM InventoryTx tx JOIN Ingredients i ON i.IngredientId = tx.IngredientId
                                  WHERE tx.IngredientId = @id
                                  ORDER BY tx.TxDate DESC";
            var dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@id", ingredientId));
            foreach (DataRow row in dt.Rows)
            {
                yield return Map(row);
            }
        }

        private static InventoryTx Map(DataRow row) => new()
        {
            TxId = (int)row["TxId"],
            IngredientId = (int)row["IngredientId"],
            TxDate = (DateTime)row["TxDate"],
            QuantityChange = (decimal)row["QuantityChange"],
            Notes = row["Notes"].ToString(),
            IngredientName = row["IngredientName"].ToString()
        };
    }
} 