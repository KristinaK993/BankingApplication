public class BankAccount : BankUser
{
    public string AccountNumber { get; set; }
    public string AccountType { get; set; } // Savings/Personal
    public decimal Balance { get; set; }
    public List<string> TransactionHistory { get; set; }
    

    public BankAccount(string accountNumber, string accountType, decimal initialBalance, string pinCode)
    {
        AccountNumber = accountNumber;
        AccountType = accountType;
        Balance = initialBalance;
        TransactionHistory = new List<string>();
    }

    public void Deposit(decimal amount) //insättning
    {
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be positive.");
            Balance += amount;
            TransactionHistory.Add($"Deposited {amount:C} on {DateTime.Now}");
        }
    }
    public bool ValidatePin(string enteredPin)
    {
        return PinCode == enteredPin;
    }

    public void Withdraw(decimal amount) //Uttag
    {
        if (amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.");
        if (amount > Balance)
            throw new InvalidOperationException("Insufficient balance.");
        Balance -= amount;
        TransactionHistory.Add($"Withdrew {amount:C} on {DateTime.Now}");
    }
   
    public string GetBalance() // Visa saldo
    {
        return $"{Balance:C}";
    }
  
    public string GetTransactionHistory() // Visa transaktionshistorik
    {
        if (TransactionHistory.Count == 0)
            return "No transactions available.";
        return string.Join(Environment.NewLine, TransactionHistory);
    }
}
