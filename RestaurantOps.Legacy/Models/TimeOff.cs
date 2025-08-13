using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOps.Legacy.Models
{
    public class TimeOff
    {
        public int TimeOffId { get; set; }
        public int EmployeeId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Denied

        // Convenience
        public string? EmployeeName { get; set; }
    }
} 