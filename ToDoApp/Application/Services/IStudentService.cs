using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Application.Dtos;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface IStudentService
    {
        IEnumerable<StudentViewModel> GetStudents(int? schoolId);

        int PostStudent(StudentCreateModel student);

        int PutStudent(StudentUpdateModel student);

        void DeleteStudent(int studentId);
    }

    public class StudentService : IStudentService
    {
        private readonly IApplicationDBContext _context;

        public StudentService(IApplicationDBContext context)
        {
            _context = context;
        }

        // IQueryable: thể hiện 1 câu query
        public IEnumerable<StudentViewModel> GetStudents(int? schoolId)
        {
            // query =
            // SELECT * FROM Student
            // JOIN School ON School.Id = Student.SId
            var query = _context.Student
                .Include(student => student.School)
                .AsQueryable(); //build 1 câu query có khả năng mở rộng

            if (schoolId.HasValue) //schoolId != null
            {
                // add thêm điều kiện vào câu query
                // query =
                // SELECT * FROM Student
                // JOIN School ON School.Id = Student.SId
                // WHERE School.Id = schoolId
                query = query
                    .Where(student => student.School.Id == schoolId.Value); 
            }

            // query =
            // SELECT 
            //  Student.Id, Student.FirstName + ' ' + Student.LastName AS FullName,
            //  Student.Age, School.Name AS SchoolName
            // FROM Student
            // JOIN School ON School.Id = Student.SId
            // WHERE School.Id = schoolId (nếu thoả điều kiện vào được body của dòng if)
            return query
                .Select(x => new StudentViewModel
                {
                    Id = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    Age = x.Age,
                    SchoolName = x.School.Name
                })         
                .ToList(); 
            //trước .ToList() là trên memory hết
            //sau khi .ToList() mới đem đoạn này đem đi execute dưới SQL
        }

        public int PostStudent(StudentCreateModel student)
        {
            if(_context.Student.Any(x =>  x.Id == student.Id))
            {
                Console.WriteLine($"Id {student.Id} already exists.");
                return student.Id;
            }

            var data = new Student
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth,
                Address1 = student.Address1,
                SId = student.SId,
            };

            _context.Student.Add(data);
            _context.SaveChanges();

            return data.Id;
        }

        public int PutStudent(StudentUpdateModel student)
        {
            var data = _context.Student.Find(student.Id);
            if (data == null) return -1;

            if(!string.IsNullOrWhiteSpace(student.FirstName)) data.FirstName = student.FirstName;
            if(!string.IsNullOrWhiteSpace(student.LastName)) data.LastName = student.LastName;
            if(student.DateOfBirth.HasValue) data.DateOfBirth = student.DateOfBirth.Value;
            if(!string.IsNullOrWhiteSpace(student.Address1)) data.Address1 = student.Address1;
            if(student.SId.HasValue) data.SId = student.SId.Value;

            _context.SaveChanges();
            return data.Id;
        }

        public void DeleteStudent(int studentId)
        {
            var data = _context.Student.Find(studentId);
            if (data == null) return;

            _context.Student.Remove(data);
            _context.SaveChanges();
        }
    }
}
