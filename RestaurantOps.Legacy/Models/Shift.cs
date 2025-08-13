using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOps.Legacy.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        public int EmployeeId { get; set; }

        [DataType(DataType.Date)]
        public DateTime ShiftDate { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required, StringLength(30)]
        public string Role { get; set; } = string.Empty;

        // Convenience
        public string? EmployeeName { get; set; }
    }
} 