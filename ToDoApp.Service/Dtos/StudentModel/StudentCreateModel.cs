using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Application.Dtos.StudentModel
{
    public class StudentCreateModel
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address1 { get; set; }

        public string SchoolName { get; set; }
        public List<string> Email { get; set; }
        public Address Address { get; set; }
    }
}
public class Address
{
    public string Street { get; set; }
    public string ZipCode { get; set; }
}