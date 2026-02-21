using FluentValidation;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Common.Enums;

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid task status");
    }
}
