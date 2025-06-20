using ToDoApp.Application.Dtos.StudentModel;

namespace ToDoApp.Application.Dtos.SchoolModel
{
    public class SchoolStudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public IEnumerable<StudentViewModel> Students { get; set; }
    }
}
