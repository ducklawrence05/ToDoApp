using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Dtos.ExamModel;
using ToDoApp.Application.Dtos.ExamQuestionModel;
using ToDoApp.DataAccess.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface IExamService
    {
        ExamViewModel PostExam(ExamCreateModel model);
        ExamViewModel PostExamQuestions(int examId, IEnumerable<ExamQuestionCreateModel> questionBankIds);
        ExamViewModel PutExamQuestions(int examId, IEnumerable<ExamQuestionCreateModel> questionBankIds);
    }

    public class ExamService : IExamService
    {
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;

        public ExamService(IApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ExamViewModel PostExam(ExamCreateModel model)
        {
            if (model == null || !_context.Courses.Any(x => x.Id == model.CourseId))
                return null;

            var data = _mapper.Map<Exam>(model);

            _context.Exams.Add(data);
            _context.SaveChanges();

            return _mapper.Map<ExamViewModel>(data);
        }

        public ExamViewModel PostExamQuestions(int examId, IEnumerable<ExamQuestionCreateModel> questionBankIds)
        {
            var exam = _context.Exams.Find(examId);
            if (exam == null) return null;
            
            var existingQuestionIds = _context.ExamQuestions
                .Where(x => x.ExamId == examId)
                .Select(x => x.QuestionBankId)
                .ToList();

            var newQuestionIds = questionBankIds
                .Select(x => x.QuestionBankId)
                .Except(existingQuestionIds)
                .ToList();

            var validQuestions = _context.QuestionBanks
                .AsNoTracking()
                .Where(x => x.CourseId == exam.CourseId && newQuestionIds.Contains(x.Id))
                .ToList();

            if (validQuestions.Count == 0) return null;

            var examQuestions = validQuestions.Select(x => new ExamQuestion
            {
                ExamId = examId,
                QuestionBankId = x.Id
            });

            _context.ExamQuestions.AddRange(examQuestions);
            _context.SaveChanges();

            var query = _context.Exams.Where(x => x.Id == examId);
            return _mapper.ProjectTo<ExamViewModel>(query).FirstOrDefault();
        }

        public ExamViewModel PutExamQuestions(int examId, IEnumerable<ExamQuestionCreateModel> questionBankIds)
        {
            var exam = _context.Exams.Find(examId);
            if (exam == null) return null;

            var questionBankIdSet = questionBankIds.Select(q => q.QuestionBankId).ToList();

            var validQuestions = _context.QuestionBanks
                .AsNoTracking()
                .Where(q => q.CourseId == exam.CourseId && questionBankIdSet.Contains(q.Id))
                .ToList();

            if (validQuestions.Count == 0) return null;

            //delete old
            var existingQuestions = _context.ExamQuestions.Where(x => x.ExamId == examId);
            _context.ExamQuestions.RemoveRange(existingQuestions);

            var examQuestions = validQuestions.Select(x => new ExamQuestion
            {
                ExamId = examId,
                QuestionBankId = x.Id
            });

            _context.ExamQuestions.AddRange(examQuestions);
            _context.SaveChanges();

            var query = _context.Exams.Where(x => x.Id == examId);
            return _mapper.ProjectTo<ExamViewModel>(query).FirstOrDefault();
        }
    }
}
