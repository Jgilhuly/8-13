using System;
using System.Collections.Generic;

namespace RestaurantOps.Legacy.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int TableId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string Status { get; set; } = "Open";

        public List<OrderLine> Lines { get; set; } = new();
    }
} 