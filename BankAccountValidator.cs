
using FluentValidation;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(account => account.HolderName)
            .NotEmpty().WithMessage("Holder name cannot be empty.")
            .MinimumLength(3).WithMessage("Holder name must be at least 3 characters long.");

        RuleFor(account => account.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Initial balance cannot be negative.");

        RuleFor(account => account.PinCode)
            .NotEmpty().WithMessage("PIN code cannot be empty.")
            .Matches(@"^\d{4}$").WithMessage("PIN code must be exactly 4 digits.");
    }
}
