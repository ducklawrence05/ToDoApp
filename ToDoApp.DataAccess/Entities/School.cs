using System.ComponentModel.DataAnnotations.Schema;
using ToDoApp.Domains.Interface;

namespace ToDoApp.Domains.Entities
{
    public class School : IUpdatedBy, IUpdatedAt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
