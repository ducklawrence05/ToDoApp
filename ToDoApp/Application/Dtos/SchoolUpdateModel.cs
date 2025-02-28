﻿using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.Dtos
{
    public class SchoolUpdateModel
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
