using AutoMapper;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.Service.MapperProfiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseViewModel>();
        }
    }
}
