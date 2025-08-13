namespace RestaurantOps.Legacy.Models
{
    public class RestaurantTable
    {
        public int TableId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Seats { get; set; }
        public bool IsOccupied { get; set; }
    }
} 