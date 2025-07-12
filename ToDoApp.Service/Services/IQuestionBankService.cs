using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Dtos.QuestionBankModel;
using ToDoApp.DataAccess.Entities;
using ToDoApp.Infrastructures;

namespace ToDoApp.Application.Services
{
    public interface IQuestionBankService
    {
        IEnumerable<QuestionBankViewModel> GetQuestions(int? courseId);

        QuestionBankViewModel PostQuestion(QuestionBankCreateModel question);

        QuestionBankViewModel PutQuestion(QuestionBankUpdateModel question);

        void DeleteQuestion(int questionBankId);
    }

    public class QuestionBankService : IQuestionBankService
    {
        private readonly IApplicationDBContext _context;
        private readonly IMapper _mapper;

        public QuestionBankService(IApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<QuestionBankViewModel> GetQuestions(int? courseId)
        {
            var query = _context.QuestionBanks.AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(x => x.CourseId == courseId);
            }

            var result = _mapper.ProjectTo<QuestionBankViewModel>(query).ToList();
            return result;
        }

        public QuestionBankViewModel PostQuestion(QuestionBankCreateModel question)
        {
            if (question == null || !_context.Courses.Any(x => x.Id == question.CourseId)) 
                return null;
            
            var data = _mapper.Map<QuestionBank>(question);
            
            _context.QuestionBanks.Add(data);
            _context.SaveChanges();

            return _mapper.Map<QuestionBankViewModel>(data);
        }

        public QuestionBankViewModel PutQuestion(QuestionBankUpdateModel question)
        {
            var data = _context.QuestionBanks.Find(question.QuestionBankId);
            if (data == null) return null;

            _mapper.Map(question, data);

            _context.SaveChanges();
            return _mapper.Map<QuestionBankViewModel>(data);
        }

        public void DeleteQuestion(int questionBankId)
        {
            var data = _context.QuestionBanks.Find(questionBankId);
            if (data == null) return;
            _context.QuestionBanks.Remove(data);
            _context.SaveChanges();
        }
    }
}
