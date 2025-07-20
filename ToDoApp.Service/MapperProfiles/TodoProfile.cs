using AutoMapper;
using Google.Apis.Auth;
using ToDoApp.Application.Dtos.CourseModel;
using ToDoApp.Application.Dtos.ExamModel;
using ToDoApp.Application.Dtos.ExamQuestionModel;
using ToDoApp.Application.Dtos.QuestionBankModel;
using ToDoApp.Application.Dtos.StudentExamAnswerModel;
using ToDoApp.Application.Dtos.StudentExamModel;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Application.Dtos.UserModel;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.Service.MapperProfiles
{
    public class TodoProfile : Profile
    {
        public TodoProfile()
        {
            // .Ignore() nếu ko muốn map prop qua nhau
            // nếu .ReverseMap() thì như tên sẽ thêm khả năng map 2 chiều
            //  nhưng nếu dùng PreCondition thì ko ReverseMap được

            // Map Course to CourseViewModel
            CreateMap<Course, CourseViewModel>()
                .ForMember
                (
                    dest => dest.CourseId, // từ đích (CourseViewModel) trỏ tới CourseId của nó
                    config => config.MapFrom(src => src.Id) //CourseId sẽ được map từ nguồn (Course) bằng Id của nguồn
                )
                .ForMember(dest => dest.CourseName, config => config.MapFrom(src => src.Name));

            // Map CourseCreateModel to Course
            CreateMap<CourseCreateModel, Course>()
                .ForMember(dest => dest.Name, x => x.MapFrom(src => src.CourseName));

            // Map CourseUpdateModel to Course
            CreateMap<CourseUpdateModel, Course>()
                .ForMember(dest => dest.Id, x => x.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.Name, config =>
                {
                    config.PreCondition(src => !string.IsNullOrWhiteSpace(src.CourseName));
                    config.MapFrom(src => src.CourseName);
                })
                .ForMember(dest => dest.StartDate, config =>
                {
                    config.PreCondition(src => src.StartDate.HasValue);
                    config.MapFrom(src => src.StartDate);
                });

            // Map Student to StudentViewModel
            CreateMap<Student, StudentViewModel>()
                .ForMember(dest => dest.FullName, config => config.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.SchoolName, config => config.MapFrom(src => src.School.Name));

            // Map CourseStudents to CourseScoreViewModel
            CreateMap<CourseStudent, CourseScoreViewModel>()
                .ForMember(dest => dest.CourseName, config => config.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.StartDate, config => config.MapFrom(src => src.Course.StartDate));

            // Map Student to StudentCourseViewModel
            CreateMap<Student, StudentCourseViewModel>()
                .ForMember(dest => dest.StudentId, config => config.MapFrom(src => src.Id))
                .ForMember(dest => dest.StudentName, config => config.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Courses, config => config.MapFrom(src => src.CourseStudents));

            // Map QuestionBank to QuestionBankViewModel
            CreateMap<QuestionBank, QuestionBankViewModel>()
                .ForMember(dest => dest.QuestionBankId, config => config.MapFrom(src => src.Id));

            // Map QuestionBankCreateModel to QuestionBank
            CreateMap<QuestionBankCreateModel, QuestionBank>();

            // Map QuestionBankUpdateModel to QuestionBank
            CreateMap<QuestionBankUpdateModel, QuestionBank>()
                .ForMember(dest => dest.Id, config => config.MapFrom(src => src.QuestionBankId))
                .ForAllMembers(config =>
                    config.Condition((src, dest, srcMember) =>
                        srcMember is not string str || !string.IsNullOrWhiteSpace(str)
                    ));

            // Map ExamCreateModel to Exam
            CreateMap<ExamCreateModel, Exam>();

            // Map QuestionBank to ExamQuestionViewModel
            CreateMap<QuestionBank, ExamQuestionViewModel>()
                .ForMember(d => d.QuestionBankId, cf => cf.MapFrom(s => s.Id));

            // Map Exam to ExamViewModel
            CreateMap<Exam, ExamViewModel>()
                .ForMember(d => d.ExamId, cf => cf.MapFrom(s => s.Id))
                .ForMember(d => d.ExamQuestions, cf => cf.MapFrom(s => s.ExamQuestions.Select(eq => eq.QuestionBank)));

            // Map StudentExamCreateModel to StudentExam
            CreateMap<StudentExamCreateModel, StudentExam>();

            // Map StudentExamAnswer to StudentExamAnswerViewModel
            CreateMap<StudentExamAnswer, StudentExamAnswerViewModel>()
                .ForMember(d => d.QuestionBankId, cf => cf.MapFrom(s => s.ExamQuestion.QuestionBankId))
                .ForMember(d => d.QuestionText, cf => cf.MapFrom(s => s.ExamQuestion.QuestionBank.QuestionText));

            // Map StudentExam to StudentExamViewModel
            CreateMap<StudentExam, StudentExamViewModel>()
                .ForMember(d => d.StudentExamId, cf => cf.MapFrom(s => s.Id))
                .ForMember(d => d.StudentExamAnswers, cf => cf.MapFrom(s => s.Answers));

            // Map UserRegisterModel to User
            CreateMap<UserRegisterModel, User>();

            CreateMap<GoogleJsonWebSignature.Payload, User>()
                .ForMember(d => d.EmailAddress, cf => cf.MapFrom(s => s.Email))
                .ForMember(d => d.FirstName, cf => cf.MapFrom(s => s.GivenName))
                .ForMember(d => d.LastName, cf => cf.MapFrom(s => s.FamilyName));
        }
    }
}
