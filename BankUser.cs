public class BankUser
{
    public string UserName { get; set; }
    public string PinCode { get; set; }
    public List<BankAccount> Accounts { get; set; }

    public BankUser()
    {
        Accounts = new List<BankAccount>();
    }

    public void AddAccount(BankAccount account)
    {
        Accounts.Add(account); 
    }

    public bool ValidatePin(string enteredPin)
    {
        return PinCode == enteredPin;
    }
}

