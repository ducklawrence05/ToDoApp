using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToDoApp.Application.Dtos.SchoolModel
{
    public class SchoolCreateModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
