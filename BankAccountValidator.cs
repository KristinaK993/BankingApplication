using FluentValidation;

public class BankUserValidator : AbstractValidator<BankUser>
{
    public BankUserValidator()
    {
        // Validera användarnamn
        RuleFor(user => user.UserName)
            .NotEmpty().WithMessage("Holder name cannot be empty.")
            .MaximumLength(50).WithMessage("Holder name must not exceed 50 characters.");

        // Validera PIN-kod
        RuleFor(user => user.PinCode)
            .NotEmpty().WithMessage("PIN code cannot be empty.")
            .Length(4).WithMessage("PIN code must be exactly 4 digits.");
    }
    public class BankAccountValidator : AbstractValidator<BankAccount>
    {
        public BankAccountValidator()
        {
            // Validera konto nummer
            RuleFor(account => account.AccountNumber)
                .NotEmpty().WithMessage("Account number cannot be empty.")
                .Length(8).WithMessage("Account number must be 8 characters long.");

            // Validera saldo
            RuleFor(account => account.Balance)
                .GreaterThanOrEqualTo(0).WithMessage("Balance cannot be negative.");
        }
    }
}
