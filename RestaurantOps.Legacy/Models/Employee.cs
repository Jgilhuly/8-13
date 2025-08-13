using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantOps.Legacy.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(30)]
        public string Role { get; set; } = string.Empty; // Server, Cook, etc.

        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Convenience
        public string FullName => $"{FirstName} {LastName}";
    }
} 