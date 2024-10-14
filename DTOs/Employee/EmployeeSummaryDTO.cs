using SP_000.Helpers;
using SP_000.Models;

namespace SP_000.DTOs.Employee
{
    public class EmployeeSummaryDTO : SearchQuery
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public DateOnly JoinedDate { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}