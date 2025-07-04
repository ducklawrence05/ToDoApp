using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using ToDoApp.Application.Dtos;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.DataAccess.Repositories;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseViewModel>> GetCourses();
        Task<int> PostCourse(CourseCreateModel course);
        Task<int> PutCourse(CourseUpdateModel course);
        Task<int> DeleteCourse(int id);
    }

    public class CourseService : ICourseService
    {
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;

        public CourseService(IApplicationDBContext context, IMapper mapper,
            ICourseRepository courseRepository)
        {
            _context = context;
            _mapper = mapper;
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<CourseViewModel>> GetCourses()
        {
            var courses = await _courseRepository.GetCoursesAsync();

            //cach 1
            //var courses = query.ToList();
            //var result = courses
            //    .Select(course => _mapper.Map<CourseViewModel>(course))
            //    .ToList();
            //var result = _mapper.Map<List<CourseViewModel>>(courses);

            //cach 2
            return _mapper.Map<List<CourseViewModel>>(courses);
        }

        public async Task<int> PostCourse(CourseCreateModel course)
        {
            if(course == null || await _courseRepository.GetCourseByNameAsync(course.CourseName) != null)
            {
                return -1;
            }

            var newCourse = _mapper.Map<Course>(course);

            return await _courseRepository.AddAsync(newCourse);
        }

        public async Task<int> PutCourse(CourseUpdateModel course)
        {
            var data = await _courseRepository.GetCourseByIdAsync(course.CourseId);
            
            if (data == null || data.DeletedBy.HasValue)
            {
                throw new InvalidOperationException("Course not found or has been deleted.");
            }

            var dupCourseName = await _courseRepository.GetCourseByNameAsync(data.Name);

            if (dupCourseName != null)
            {
                throw new Exception("Couse name is duplicated");
            }

            //if(!string.IsNullOrWhiteSpace(course.CourseName)) data.Name = course.CourseName;
            //if(course.StartDate.HasValue) data.StartDate = course.StartDate.Value;

            _mapper.Map(course, data); // map từ src (course) về dest (data), ko tạo mới
                                       //   còn cái .Map<>() là tạo mới

            await _courseRepository.UpdateAsync(data);
            return data.Id;
        }

        public async Task<int> DeleteCourse(int id)
        {
            return await _courseRepository.DeleteAsync(id);
        }
    }
}
