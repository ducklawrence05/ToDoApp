using System.ComponentModel.DataAnnotations.Schema;
using ToDoApp.DataAccess.Interface;

namespace ToDoApp.DataAccess.Entities
{
    public class School : IEntity, IUpdatedBy, IUpdatedAt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
