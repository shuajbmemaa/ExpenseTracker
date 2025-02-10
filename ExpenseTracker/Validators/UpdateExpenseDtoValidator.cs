using ExpenseTracker.DTOs;
using FluentValidation;

namespace ExpenseTracker.Validators;

public class UpdateExpenseDtoValidator : AbstractValidator<UpdateExpenseDto>
{
    public UpdateExpenseDtoValidator()
    {
        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description, if provided, cannot be empty.");
        });

        When(x => x.Amount.HasValue, () =>
        {
            RuleFor(x => x.Amount.Value)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        });

        When(x => x.Date != null, () =>
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date, if provided, is required.");
        });

        When(x => x.CategoryId.HasValue, () =>
        {
            RuleFor(x => x.CategoryId.Value)
                .GreaterThan(0).WithMessage("A valid category must be selected.");
        });
    }
}
