using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface ICourseService
    {
        CourseStudentViewModel GetCourseDetail(int id);
        IEnumerable<CourseViewModel> GetCourses();
        Task<CourseViewModel> PostCourse(CourseCreateModel course);
        CourseViewModel PutCourse(CourseUpdateModel course);
        void DeleteCourse(int id);
    }

    public class CourseService : ICourseService
    {
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;

        public CourseService(IApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public CourseStudentViewModel GetCourseDetail(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return null;

            var students = _context.CourseStudents
                .Where(x => x.CourseId == id)
                .Select(x => new StudentViewModel
                {
                    Id = x.StudentId,
                    FullName = x.Student.FirstName + " " + x.Student.LastName,
                    Age = x.Student.Age,
                    SchoolName = x.Student.School.Name
                });

            return new CourseStudentViewModel
            {
                CourseId = course.Id,
                CourseName = course.Name,
                StartDate = course.StartDate,
                Students = students.ToList()
            };
        }

        public IEnumerable<CourseViewModel> GetCourses()
        {
            var query = _context.Courses.AsQueryable();

            //cach 1
            //var courses = query.ToList();
            //var result = courses
            //    .Select(course => _mapper.Map<CourseViewModel>(course))
            //    .ToList();
            //var result = _mapper.Map<List<CourseViewModel>>(courses);

            //cach 2
            var result = _mapper.ProjectTo<CourseViewModel>(query).ToList();

            return result;
        }

        public async Task<CourseViewModel> PostCourse(CourseCreateModel course)
        {
            if(course == null || await _context.Courses.AnyAsync(x => x.Name == course.CourseName))
            {
                return null;
            }

            var newCourse = _mapper.Map<Course>(course);

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return _mapper.Map<CourseViewModel>(newCourse);
        }

        public CourseViewModel PutCourse(CourseUpdateModel course)
        {
            var data = _context.Courses.Find(course.CourseId);
            if (data == null) return null;

            //if(!string.IsNullOrWhiteSpace(course.CourseName)) data.Name = course.CourseName;
            //if(course.StartDate.HasValue) data.StartDate = course.StartDate.Value;

            _mapper.Map(course, data); // map từ src (course) về dest (data), ko tạo mới
                                       //   còn cái .Map<>() là tạo mới

            _context.SaveChanges();
            return _mapper.Map<CourseViewModel>(data);
        }

        public void DeleteCourse(int id)
        {
            var data = _context.Courses.Find(id);
            if (data == null) return;
            _context.Courses.Remove(data);
            _context.SaveChanges();
        }
    }
}
