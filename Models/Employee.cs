using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP_000.Models
{
    public class Employee : Metadata
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? EmployeeId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public DateOnly JoinedDate { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public enum Gender
    {
        [Description("Male")]
        Male,
        Female
    }
}