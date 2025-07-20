using FluentValidation;
using ToDoApp.Application.Dtos.StudentModel;
using ToDoApp.Infrastructures;

namespace ToDoApp.ModelValidation
{
    public class StudentCreateModelValidator : AbstractValidator<StudentCreateModel>
    {
        private readonly IApplicationDBContext _dbContext;

        public StudentCreateModelValidator(IApplicationDBContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required")
                .Must(DoesNotExist)
                .WithMessage("Id can not be duplicate");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("FirstName is required")
                .Length(2, 50)
                .WithMessage("FirstName must be between 2 and 50 characters long.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("LastName is required");

            RuleFor(x => x.SchoolName)
                .NotEmpty()
                .WithMessage("School name is required");

            RuleForEach(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Address)
                .SetValidator(new AddressValidator())
                .WithMessage("Invalid address format");
        }

        private bool DoesNotExist(int id)
        {
            return !_dbContext.Students.Any(x => x.Id == id);
        }
    }
}
