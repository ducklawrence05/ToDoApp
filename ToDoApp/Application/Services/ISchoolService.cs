using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Dtos.SchoolModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface ISchoolService
    {
        IEnumerable<SchoolViewModel> GetSchools(string? address);

        SchoolStudentViewModel GetSchoolDetail(int schoolId);
        
        int PostSchool(SchoolCreateModel school);

        int PutSchool(SchoolUpdateModel school);

        void DeleteSchool(int schoolId);
    }

    public class SchoolService : ISchoolService
    {
        private readonly IApplicationDBContext _context;
        
        public SchoolService(IApplicationDBContext context)
        {
            _context = context;
        }

        public IEnumerable<SchoolViewModel> GetSchools(string? address)
        {
            var query = _context.Schools.AsQueryable();

            if (!string.IsNullOrEmpty(address))
            {
                query = query.Where(x => x.Address.Contains(address));
            }
            
            return query
                .Select(x => new SchoolViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address
                })
                .ToList();
        }

        public SchoolStudentViewModel GetSchoolDetail(int schoolId)
        {
            // 1 school => 10 students
            // 1 student => 3 courses
            var school = _context.Schools.Find(schoolId); // 1 câu query
            if(school == null)
            {
                return null;
            }

            //Explixit loading: khi nào .Load() thì nó sẽ load, chứ nó y rang thằng Lazy loading
            _context.Entry(school).Collection(x => x.Students).Load();

            var students = school.Students; // 1 câu query

            //foreach(var student in students) // 10 câu query
            //{
            //    var courseStudents = student.CourseStudents; // 10 câu query
            //    foreach(var courseStudent in courseStudents) 
            //    {
            //        var course = courseStudent.Course;
            //    } //10 câu query
            //}

            return new SchoolStudentViewModel
            {
                Id = school.Id,
                Name = school.Name,
                Address = school.Address,
                Students = students.Select(students => new StudentViewModel
                {
                    Id = students.Id,
                    FullName = students.FirstName + " " + students.LastName,
                    Age = students.Age,
                    SchoolName = students.School.Name
                }).ToList()
            };
        }

        public int PostSchool(SchoolCreateModel school)
        {
            var data = new School
            {
                Name = school.Name,
                Address = school.Address
            };

            var state = _context.Entry(data).State; //Detached

            _context.Schools.Add(data); //Added, bản thân .Add() là chuyển state sang Added, tương tự Modified
            //_context.Entry(data).State = EntityState.Added;
            //tăng hiệu suất app thì thêm .AsNoTracking(), lúc này EF sẽ ko track nữa, state sẽ là Detached
            //  khi SaveChanges cũng ko xuống db => dùng khi chỉ muốn query

            state = _context.Entry(data).State;

            _context.SaveChanges();
            return data.Id;
        }

        public int PutSchool(SchoolUpdateModel school)
        {
            var data = _context.Schools.Find(school.Id);
            if (data == null) return -1;

            if(!string.IsNullOrWhiteSpace(school.Name)) data.Name = school.Name;
            if(!string.IsNullOrWhiteSpace(school.Address)) data.Address = school.Address;

            _context.SaveChanges();
            return data.Id;
        }

        public void DeleteSchool(int schoolId)
        {
            var data = _context.Schools.Find(schoolId);
            if (data == null) return;

            _context.Schools.Remove(data);
            _context.SaveChanges();
        }
    }
}
