using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOps.Legacy.Models
{
    public class InventoryTx
    {
        public int TxId { get; set; }
        public int IngredientId { get; set; }

        public DateTime TxDate { get; set; }

        [Range(-99999, 99999)]
        public decimal QuantityChange { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        // Convenience display property
        public string? IngredientName { get; set; }
    }
} 