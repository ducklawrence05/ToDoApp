using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ToDoApp.Application.Dtos.SchoolModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.DataAccess.Entities;
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

        Task<ImportResult> ImportSchools(IFormFile file);
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

        public async Task<ImportResult> ImportSchools(IFormFile file)
        {
            using var excelFile = new ExcelPackage(file.OpenReadStream());
            var workSheet = excelFile.Workbook.Worksheets.FirstOrDefault();

            if (workSheet == null)
            {
                return new ImportResult
                {
                    IsSuccess = false,
                    Message = "No worksheet found in the provided Excel file."
                };
            }

            // Validate
            // row, col: error msg
            // Example: row 1, col 1, schoolId does not exist

            // Update when sId not empty
            // Create new when sId empty

            string message = "Successfully";
            var rowCount = workSheet.Dimension.Rows;
            var schools = _context.Schools.ToList();

            for (int i = 2; i <= rowCount; i++)
            {
                var idRaw = workSheet.Cells[i, 1].Text.Trim();
                var schoolName = workSheet.Cells[i, 2].Text.Trim();
                var schoolAdress = workSheet.Cells[i, 3].Text.Trim();

                if (int.TryParse(idRaw, out var id))
                {
                    var school = schools.FirstOrDefault(x => x.Id == id);
                    if (school == null)
                    {
                        return new ImportResult
                        {
                            IsSuccess = false,
                            Message = $"row {i}, cell {1}: schoolId does not exist"
                        };
                    }

                    school.Name = schoolName;
                    school.Address = schoolAdress;
                }
                else
                {
                    _context.Schools.Add(new School
                    {
                        Name = schoolName,
                        Address = schoolAdress
                    });
                }
            }

            _context.SaveChangesAsync();
            return new ImportResult
            {
                IsSuccess = true,
                Message = message
            };
        }
    }
}
