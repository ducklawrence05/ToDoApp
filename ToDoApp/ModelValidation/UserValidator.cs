using FluentValidation;
using ToDoApp.Application.Dtos.UserModel;

namespace ToDoApp.ModelValidation
{
    public class UserValidator : AbstractValidator<UserRegisterModel>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username is required");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Confirm password is not matched password");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required");

            RuleFor(x => x.LastName)
                .NotEmpty().
                WithMessage("Last name is required");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("Email address is required")
                .EmailAddress()
                .WithMessage("Email address format is invalid");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Role is invalid");
        }
    }
}
