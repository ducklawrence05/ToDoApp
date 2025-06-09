using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.Dtos.StudentModel
{
    public class StudentUpdateModel
    {
        [Required]
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address1 { get; set; }

        public int? SId { get; set; }

        public decimal Balance { get; set; }
    }
}
