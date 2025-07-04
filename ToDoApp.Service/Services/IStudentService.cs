using System.Buffers;
using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Application.Extentions;
using ToDoApp.DataAccess.Repositories;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface IStudentService
    {
        StudentCourseViewModel GetStudentDetail(int id);

        StudentAverageScoreViewModel GetStudentAverageScore(int id);

        IEnumerable<StudentViewModel> GetStudents(
            string? searchProperty, string? searchValue,
            string? sortBy, bool isAscending,
            int pageIndex, int pageSize
        );

        Task<IEnumerable<StudentViewModel>> GetStudentsAsync();

        int PostStudent(StudentCreateModel student);

        int PutStudent(StudentUpdateModel student);

        void DeleteStudent(int studentId);
    }

    public class StudentService : IStudentService
    {
        private const string STUDENT_KEY = "StudentKey";
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IStudentRepository _studentRepository;

        public StudentService(IApplicationDBContext context, 
            IMapper mapper, IMemoryCache cache, IStudentRepository studentRepository)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
            _studentRepository = studentRepository;
        }

        public StudentCourseViewModel GetStudentDetail(int id)
        {
            var student = _context.Students
                .Where(x => x.Id == id)
                .Include(x => x.CourseStudents)
                .ThenInclude(x => x.Course)
                .FirstOrDefault(); //FoD cũng sẽ execute giống ToList()

            if (student == null) return null;

            var result = _mapper.Map<StudentCourseViewModel>(student);

            return result;

            //return new StudentCourseViewModel
            //{
            //    StudentId = student.Id,
            //    StudentName = student.FirstName + " " + student.LastName,
            //    Courses = student.CourseStudents
            //    .Select(x => new CourseScoreViewModel
            //    {
            //        CourseId = x.Course.Id,
            //        CourseName = x.Course.Name,
            //        StartDate = x.Course.StartDate,
            //        AssignmentScore = x.AssignmentScore,
            //        PracticalScore = x.PracticalScore,
            //        FinalScore = x.FinalScore
            //    })
            //    .ToList()
            //};
        }

        public StudentAverageScoreViewModel GetStudentAverageScore(int id)
        {
            var student = _context.Students
                .Where(x => x.Id == id)
                .Include(x => x.CourseStudents)
                .FirstOrDefault();

            if (student == null) return null;

            return new StudentAverageScoreViewModel
            {
                Id = student.Id,
                FullName = student.FirstName + " " + student.LastName,
                AverageScore = student.CourseStudents
                    .Average(x => new[]
                    {
                        x.AssignmentScore,
                        x.PracticalScore,
                        x.FinalScore
                    }.Average())
            };
        }

        // IQueryable: thể hiện 1 câu query
        public IEnumerable<StudentViewModel> GetStudents(
            string? searchProperty, string? searchValue,
            string? sortBy, bool isAscending,
            int pageIndex, int pageSize
        )
        {
            var query = _context.Students            // trả ra DbSet<Student>
                .Include(student => student.School)  // trả về IIncludeQueryable, ko chấm Where được
                .AsQueryable(); // nên mới phải cast lại thành IQueryable, để build 1 câu query có khả năng mở rộng

            query = query.ApplyQuery(searchProperty, searchValue, sortBy, isAscending, pageIndex, pageSize);

            var result = _mapper.ProjectTo<StudentViewModel>(query).ToList();

            return result;
        }

        public async Task<IEnumerable<StudentViewModel>> GetStudentsAsync()
        {
            //var data = _cache.Get<IEnumerable<StudentViewModel>>(STUDENT_KEY);

            //if(data == null)
            //{
            //    data = GetAllStudents();

            //    var cacheOption = new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

            //    _cache.Set(STUDENT_KEY, data, cacheOption);
            //}

            var data = await _cache.GetOrCreate(STUDENT_KEY, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                return await GetAllStudentsAsync();
            });

            return data;
        }

        private async Task<IEnumerable<StudentViewModel>> GetAllStudentsAsync()
        {
            var student = await _studentRepository.GetStudentsAsync(s => s.School);
            var result = _mapper.Map<List<StudentViewModel>>(student);
            return result;
        }

        public int PostStudent(StudentCreateModel student)
        {
            if(student == null || _context.Students.Any(x =>  x.Id == student.Id))
            {
                return -1;
            }

            var data = new Student
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Address1 = student.Address1,
                DateOfBirth = student.DateOfBirth,
                SId = student.SId,
            };

            _context.Students.Add(data);
            _context.SaveChanges();

            return data.Id;
        }

        public int PutStudent(StudentUpdateModel student)
        {
            var data = _context.Students.Find(student.Id);
            if (data == null) return -1;

            if(!string.IsNullOrWhiteSpace(student.FirstName)) data.FirstName = student.FirstName;
            if(!string.IsNullOrWhiteSpace(student.LastName)) data.LastName = student.LastName;
            if(student.DateOfBirth.HasValue) data.DateOfBirth = student.DateOfBirth.Value;
            if(!string.IsNullOrWhiteSpace(student.Address1)) data.Address1 = student.Address1;
            if (student.SId.HasValue) data.SId = student.SId.Value;

            data.Balance = student.Balance;

            _context.SaveChanges();

            _cache.Remove(STUDENT_KEY);

            return data.Id;
        }

        public void DeleteStudent(int studentId)
        {
            var data = _context.Students.Find(studentId);
            if (data == null) return;

            _context.Students.Remove(data);
            _context.SaveChanges();
        }
    }
}
