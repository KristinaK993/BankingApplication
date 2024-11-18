using BankingApplication;
using FluentValidation;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(account => account.AccountNumber)
            .NotEmpty().WithMessage("Account number cannot be empty.")
            .Length(8).WithMessage("Account number must be 8 characters long.");

        RuleFor(account => account.HolderName)
            .NotEmpty().WithMessage("Holder name cannot be empty.")
            .MaximumLength(50).WithMessage("Holder name must not exceed 50 characters.");

        RuleFor(account => account.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance cannot be negative.");

        RuleFor(account => account.PinCode)
            .NotEmpty().WithMessage("PIN code cannot be empty.")
            .Length(4).WithMessage("PIN code must be exactly 4 digits.");
    }
}
