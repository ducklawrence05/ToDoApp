namespace ToDoApp.Application.Dtos.EnrollmentModel
{
    public class EnrollmentCreateModel
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }
        public double? AssignmentScore { get; set; }
        public double? PracticalScore { get; set; }
        public double? FinalScore { get; set; }
    }
}
