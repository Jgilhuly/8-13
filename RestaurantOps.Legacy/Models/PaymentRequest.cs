using System.ComponentModel.DataAnnotations;

namespace RestaurantOps.Legacy.Models
{
    public class PaymentRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Amount { get; set; }

        public string? CardLast4 { get; set; }
    }
} 