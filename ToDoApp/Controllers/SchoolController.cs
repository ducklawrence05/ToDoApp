using Microsoft.AspNetCore.Mvc;
using ToDoApp.Application.Services;
using Microsoft.EntityFrameworkCore.Storage;
using ToDoApp.Application.Dtos.SchoolModel;
using ToDoApp.Application.ActionFilters;
using Microsoft.AspNetCore.Authorization;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[TypeFilter(typeof(AuthorizationFilter), Arguments = [new string[] {"Admin", "Staff"}])]

    [Authorize(Roles = "Admin,Staff")]
    // hoặc truyền 1 chuỗi ròi băm ra
    // $"{nameof(Role.Admin)},{nameof(Role.Staff)}"
    [Authorize]
    [TypeFilter(typeof(TokenBlackListFilter))]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;
        
        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        [HttpGet]
        public IEnumerable<SchoolViewModel> GetSchools(string? address)
        {
            return _schoolService.GetSchools(address);
        }

        [HttpGet("{id}/detail")]
        public SchoolStudentViewModel GetSchoolDetail(int id)
        {
            //var userId = HttpContext.Session.GetInt32("UserId");
            //var role = HttpContext.Session.GetString("Role");

            //if (userId == null || role != "Admin")
            //{
            //    return null;
            //}

            return _schoolService.GetSchoolDetail(id);
        }

        [HttpPost]
        public int PostSchool(SchoolCreateModel school)
        {
            return _schoolService.PostSchool(school);
        }

        [HttpPut]
        public int PutSchool(SchoolUpdateModel school)
        {
            return _schoolService.PutSchool(school);
        }

        [HttpDelete]
        public void DeleteSchool(int schoolId)
        {
            _schoolService.DeleteSchool(schoolId);
        }
    }
}
