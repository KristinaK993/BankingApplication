public class BankAccount
{
    public string AccountNumber { get; set; }
    public string AccountType { get; set; } // Savings/Personal
    public decimal Balance { get; set; }
    public List<string> TransactionHistory { get; private set; }

    public BankAccount(string accountNumber, string accountType, decimal initialBalance, string pinCode)
    {
        AccountNumber = accountNumber;
        AccountType = accountType;
        Balance = initialBalance;
        TransactionHistory = new List<string>();
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be positive.");
        }

        Balance += amount;
        string transaction = $"Deposited {amount:C} on {DateTime.Now}";
        TransactionHistory.Add(transaction); // Lägg till transaktion
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be positive.");
        }

        if (amount > Balance)
        {
            throw new InvalidOperationException("Insufficient balance.");
        }

        Balance -= amount;
        TransactionHistory.Add($"Withdrew {amount:C} on {DateTime.Now}");
    }

    public override string ToString()
    {
        return $"Account: {AccountNumber}, Type: {AccountType}, Balance: {Balance:C}";
    }
}
