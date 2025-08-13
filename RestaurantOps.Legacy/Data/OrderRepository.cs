using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
    public class OrderRepository
    {
        public Order Create(int tableId)
        {
            const string sql = "INSERT INTO Orders (TableId) OUTPUT INSERTED.OrderId VALUES (@tableId)";
            var id = (int)SqlHelper.ExecuteScalar(sql, new SqlParameter("@tableId", tableId))!;
            return new Order { OrderId = id, TableId = tableId, CreatedAt = DateTime.UtcNow };
        }

        public Order? GetCurrentByTable(int tableId)
        {
            const string sql = @"SELECT TOP 1 * FROM Orders WHERE TableId = @tableId AND Status = 'Open' ORDER BY CreatedAt DESC";
            var dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@tableId", tableId));
            if (dt.Rows.Count == 0) return null;
            var order = MapOrder(dt.Rows[0]);
            order.Lines = GetLines(order.OrderId).ToList();
            return order;
        }

        public Order? GetById(int orderId)
        {
            const string sql = "SELECT * FROM Orders WHERE OrderId = @id";
            var dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@id", orderId));
            if (dt.Rows.Count == 0) return null;
            var order = MapOrder(dt.Rows[0]);
            order.Lines = GetLines(order.OrderId).ToList();
            return order;
        }

        public void AddLine(int orderId, int menuItemId, int qty, decimal price)
        {
            const string sql = @"INSERT INTO OrderLines (OrderId, MenuItemId, Quantity, PriceEach) VALUES (@orderId, @itemId, @qty, @price)";
            SqlHelper.ExecuteNonQuery(sql,
                new SqlParameter("@orderId", orderId),
                new SqlParameter("@itemId", menuItemId),
                new SqlParameter("@qty", qty),
                new SqlParameter("@price", price));
        }

        public void CloseOrder(int orderId)
        {
            const string sql = "UPDATE Orders SET Status='Closed', ClosedAt=SYSUTCDATETIME() WHERE OrderId=@id";
            SqlHelper.ExecuteNonQuery(sql, new SqlParameter("@id", orderId));
        }

        public void SubmitOrder(int orderId)
        {
            const string sql = "UPDATE Orders SET Status='Submitted' WHERE OrderId=@id";
            SqlHelper.ExecuteNonQuery(sql, new SqlParameter("@id", orderId));
        }

        public IEnumerable<OrderLine> GetLines(int orderId)
        {
            const string sql = @"SELECT ol.OrderLineId, ol.MenuItemId, mi.Name AS MenuItemName, ol.Quantity, ol.PriceEach
                                   FROM OrderLines ol JOIN MenuItems mi ON mi.MenuItemId = ol.MenuItemId
                                   WHERE ol.OrderId = @orderId";
            var dt = SqlHelper.ExecuteDataTable(sql, new SqlParameter("@orderId", orderId));
            foreach (DataRow row in dt.Rows)
            {
                yield return MapLine(row);
            }
        }

        private static Order MapOrder(DataRow row) => new()
        {
            OrderId = (int)row["OrderId"],
            TableId = (int)row["TableId"],
            CreatedAt = (DateTime)row["CreatedAt"],
            ClosedAt = row["ClosedAt"] == DBNull.Value ? null : (DateTime)row["ClosedAt"],
            Status = row["Status"].ToString()!
        };

        private static OrderLine MapLine(DataRow row) => new()
        {
            OrderLineId = (int)row["OrderLineId"],
            MenuItemId = (int)row["MenuItemId"],
            MenuItemName = row["MenuItemName"].ToString()!,
            Quantity = (int)row["Quantity"],
            PriceEach = (decimal)row["PriceEach"]
        };
    }
} 