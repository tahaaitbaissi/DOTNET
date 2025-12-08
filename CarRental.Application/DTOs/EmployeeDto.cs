using System;

namespace CarRental.Application.DTOs
{
    public class EmployeeDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
    }
}
