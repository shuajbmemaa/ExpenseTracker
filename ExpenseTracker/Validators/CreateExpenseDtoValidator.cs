using ExpenseTracker.DTOs;
using FluentValidation;

namespace ExpenseTracker.Validators;

public class CreateExpenseDtoValidator : AbstractValidator<CreateExpenseDto>
{
    public CreateExpenseDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotNull()
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("A valid category must be selected.");
    }
}
