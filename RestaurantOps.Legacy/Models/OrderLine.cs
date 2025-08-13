namespace RestaurantOps.Legacy.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceEach { get; set; }
        public decimal LineTotal => Quantity * PriceEach;
    }
} 