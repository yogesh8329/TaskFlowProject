using FluentValidation;
using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6);

            //RuleFor(x => x.Role)
            //    .NotEmpty().WithMessage("Role is required");
        }
    }
}
