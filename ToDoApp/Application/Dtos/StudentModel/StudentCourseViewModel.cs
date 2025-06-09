using ToDoApp.Application.Dtos.CourseModel;

namespace ToDoApp.Application.Dtos.StudentModel
{
    public class StudentCourseViewModel
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public List<CourseScoreViewModel> Courses { get; set; }
    }
}
