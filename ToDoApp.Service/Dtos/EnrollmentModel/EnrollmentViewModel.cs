using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.StudentModel;

namespace ToDoApp.Application.Dtos.EnrollmentModel
{
    public class EnrollmentViewModel
    {
        public CourseViewModel Course { get; set; }
        public StudentViewModel Student { get; set; }
        public double AssignmentScore { get; set; }
        public double PracticalScore { get; set; }
        public double FinalScore { get; set; }

    }
}
