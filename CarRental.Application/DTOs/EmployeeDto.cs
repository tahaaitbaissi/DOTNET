using System;

namespace CarRental.Application.DTOs
{
    public class EmployeeDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Position { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
    }
}
