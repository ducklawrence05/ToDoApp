using System.Runtime.Intrinsics.X86;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using ToDoApp.Application.Dtos.StudentExamAnswerModel;
using ToDoApp.Application.Dtos.StudentExamModel;
using ToDoApp.Domains.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface IStudentExamService
    {
        StudentExamViewModel PostStudentExam(StudentExamCreateModel model);

        StudentExamViewModel PostStudentExamAnswers(int studentExamId, IEnumerable<StudentExamAnswerCreateModel> studentExamAnswers);
    }

    public class StudentExamService : IStudentExamService
    {
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;

        public StudentExamService(IApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public StudentExamViewModel PostStudentExam(StudentExamCreateModel model)
        {
            if (model == null) return null;

            var student = _context.Students.Find(model.StudentId);
            var exam = _context.Exams.Find(model.ExamId);

            if (student == null || exam == null) 
                return null;

            if (!_context.CourseStudents
                .Any(cs => cs.StudentId == student.Id && cs.CourseId == exam.CourseId))
                return null;

            var data = _mapper.Map<StudentExam>(model);

            _context.StudentExams.Add(data);
            _context.SaveChanges();

            return _mapper.Map<StudentExamViewModel>(data);
        }

        public StudentExamViewModel PostStudentExamAnswers(int studentExamId, IEnumerable<StudentExamAnswerCreateModel> studentExamAnswers)
        {
            //get student exam
            var studentExam = _context.StudentExams
                .Where(x => x.Id == studentExamId)
                .Include(se => se.Exam)
                    .ThenInclude(e => e.ExamQuestions)
                        .ThenInclude(eq => eq.QuestionBank)
                .FirstOrDefault();

            if (studentExam == null) return null;

            if (_context.StudentExamAnswers.Any(x => x.StudentExamId == studentExamId)) return null;

            // get exam questions and convert to Dictionary
            var examQuestions = studentExam.Exam.ExamQuestions
                .ToDictionary(eq => eq.Id, eq => eq.QuestionBank.CorrectAnswer);

            // filter duplicate and existed in exam questions
            var validAnswers = studentExamAnswers
                .GroupBy(ans => ans.ExamQuestionId)
                .Select(x => x.Last())
                .Where(dq => examQuestions.ContainsKey(dq.ExamQuestionId))
                .ToList();

            // filter correct answer
            var correctAnswersCount = validAnswers.Count(ans =>
                string.Equals(
                    ans.SelectedAnswer,
                    examQuestions[ans.ExamQuestionId],
                    StringComparison.OrdinalIgnoreCase));

            // get total questions
            var totalQuestions = examQuestions.Count;

            // calc score
            var score = totalQuestions == 0 ? 0 : (10.0 * correctAnswersCount / totalQuestions);

            // update score on student exam and course student table
            studentExam.Score = score;

            var courseStudent = _context.CourseStudents
                .FirstOrDefault(cs => cs.StudentId == studentExam.StudentId && cs.CourseId == studentExam.Exam.CourseId);

            if (courseStudent != null) courseStudent.FinalScore = score;

            var datas = validAnswers.Select(x => new StudentExamAnswer
            {
                StudentExamId = studentExamId,
                ExamQuestionId = x.ExamQuestionId,
                SelectedAnswer = x.SelectedAnswer,
            });

            _context.StudentExamAnswers.AddRange(datas);
            _context.SaveChanges();

            var query = _context.StudentExams.Where(x => x.Id == studentExamId);
            return _mapper.ProjectTo<StudentExamViewModel>(query).FirstOrDefault();
        }
    }
}
