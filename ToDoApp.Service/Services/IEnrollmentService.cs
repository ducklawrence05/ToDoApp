using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.EnrollmentModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Application.Extentions;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface IEnrollmentService
    {
        EnrollmentViewModel PostEnrollment(EnrollmentCreateModel enrollment);
        EnrollmentViewModel PutEnrollment(EnrollmentCreateModel enrollment);
    }

    public class EnrollmentService : IEnrollmentService
    {
        private readonly IApplicationDBContext _context;

        public EnrollmentService(IApplicationDBContext context)
        {
            _context = context;
        }

        public EnrollmentViewModel PostEnrollment(EnrollmentCreateModel enrollment)
        {
            var alreadyEnrolled = _context.CourseStudents
                .Any(x => x.CourseId == enrollment.CourseId && x.StudentId == enrollment.StudentId);
            if (alreadyEnrolled) return null;

            var course = _context.Courses.Find(enrollment.CourseId);
            var student = _context.Students
                .Include(x => x.School)
                .FirstOrDefault(x => x.Id == enrollment.StudentId);
            if (course == null || student == null) return null;

            var newEnrollment = new CourseStudent
            {
                CourseId = enrollment.CourseId,
                StudentId = enrollment.StudentId,
                AssignmentScore = enrollment.AssignmentScore.GetValidScore(),
                PracticalScore = enrollment.PracticalScore.GetValidScore(),
                FinalScore = enrollment.FinalScore.GetValidScore()
            };

            _context.CourseStudents.Add(newEnrollment);
            _context.SaveChanges();
            return new EnrollmentViewModel
            {
                Student = new StudentViewModel
                {
                    Id = student.Id,
                    FullName = student.FirstName + " " + student.LastName,
                    Age = student.Age,
                    SchoolName = student.School.Name
                },
                Course = new CourseViewModel
                {
                    CourseId = course.Id,
                    CourseName = course.Name,
                    StartDate = course.StartDate
                },
                AssignmentScore = newEnrollment.AssignmentScore,
                PracticalScore = newEnrollment.PracticalScore,
                FinalScore = newEnrollment.FinalScore
            };
        }

        public EnrollmentViewModel PutEnrollment(EnrollmentCreateModel enrollment)
        {
            var data = _context.CourseStudents
                .Find(enrollment.CourseId, enrollment.StudentId);
            if (data == null) return null;

            data.AssignmentScore = enrollment.AssignmentScore.GetValidScore(data.AssignmentScore);

            data.PracticalScore = enrollment.PracticalScore.GetValidScore(data.PracticalScore);

            data.FinalScore = enrollment.FinalScore.GetValidScore(data.FinalScore);

            _context.SaveChanges();
            return new EnrollmentViewModel
            {
                Student = new StudentViewModel
                {
                    Id = data.Student.Id,
                    FullName = data.Student.FirstName + " " + data.Student.LastName,
                    Age = data.Student.Age,
                    SchoolName = data.Student.School.Name
                },
                Course = new CourseViewModel
                {
                    CourseId = data.Course.Id,
                    CourseName = data.Course.Name,
                    StartDate = data.Course.StartDate
                },
                AssignmentScore = data.AssignmentScore,
                PracticalScore = data.PracticalScore,
                FinalScore = data.FinalScore
            };
        }
    }
}
