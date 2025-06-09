namespace ToDoApp.Application.Dtos.CourseModel
{
    public class CourseScoreViewModel : CourseViewModel
    {
        public double AssignmentScore { get; set; }
        public double PracticalScore { get; set; }
        public double FinalScore { get; set; }
    }
}
