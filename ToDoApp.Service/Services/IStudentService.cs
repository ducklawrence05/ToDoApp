using System.Buffers;
using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Application.Extentions;
using ToDoApp.DataAccess.Entities;
using ToDoApp.DataAccess.Repositories;
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

        Task<int> PostAsync(StudentCreateModel student);

        Task<int> PutAsync(StudentUpdateModel student);

        Task<int> DeleteAsync(int studentId);
    }

    public class StudentService : IStudentService
    {
        private const string STUDENT_KEY = "StudentKey";
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IStudentRepository _studentRepository;
        private readonly ISchoolRepository _schoolRepository;

        public StudentService(IApplicationDBContext context, 
            IMapper mapper, IMemoryCache cache, IStudentRepository studentRepository,
            ISchoolRepository schoolRepository)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
            _studentRepository = studentRepository;
            _schoolRepository = schoolRepository;
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
            var student = await _studentRepository.GetAllAsync(s => s.School);
            var result = _mapper.Map<List<StudentViewModel>>(student);
            return result;
        }

        public async Task<int> PostAsync(StudentCreateModel student)
        {
            var dupId = await _studentRepository.GetByIdAsync(student.Id);
            if(dupId != null)
            {
                throw new InvalidOperationException("Student ID already exists");
            }

            var school = _schoolRepository.GetSchoolByNameAsync(student.SchoolName);
            if (school == null)
            {
                throw new InvalidOperationException("School name not found");
            }

            var data = new Student
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Address1 = student.Address1,
                DateOfBirth = student.DateOfBirth,
                SId = school.Id,
            };

            await _studentRepository.AddAsync(data);

            return data.Id;
        }

        public async Task<int> PutAsync(StudentUpdateModel student)
        {
            var data = await _studentRepository.GetByIdAsync(student.Id);
            if (data == null)
            {
                throw new InvalidOperationException("Student not found");
            }

            if (!string.IsNullOrWhiteSpace(student.FirstName)) data.FirstName = student.FirstName;
            if(!string.IsNullOrWhiteSpace(student.LastName)) data.LastName = student.LastName;
            if(student.DateOfBirth.HasValue) data.DateOfBirth = student.DateOfBirth.Value;
            if(!string.IsNullOrWhiteSpace(student.Address1)) data.Address1 = student.Address1;
            if (student.SId.HasValue) data.SId = student.SId.Value;

            data.Balance = student.Balance;

            await _studentRepository.UpdateAsync(data);

            //_cache.Remove(STUDENT_KEY);

            return data.Id;
        }

        public async Task<int> DeleteAsync(int studentId)
        {
            var data = await _studentRepository.GetByIdAsync(studentId);
            if (data == null) {
                throw new InvalidOperationException("Student not found");
            }

            _context.Students.Remove(data);
            await _context.SaveChangesAsync();
            return data.Id;
        }
    }
}
