namespace ToDoApp.Application.Dtos.CourseModel
{
    public class CourseUpdateModel
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
