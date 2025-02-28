using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.Dtos
{
    public class StudentCreateModel
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(255)]
        public string? FirstName { get; set; }

        [StringLength(255)]
        public string? LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address1 { get; set; }

        public int SId { get; set; }
    }
}
