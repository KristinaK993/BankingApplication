using System;
using System.Collections.Generic;

namespace BankingApplication
{
    public class BankAccount
    {
        public string AccountNumber { get; set; }
        public string HolderName { get; set; }
        public decimal Balance { get; set; }
        public string PinCode { get; set; }
        public List<string> TransactionHistory { get; set; } = new List<string>();

        public BankAccount(string accountNumber, string holderName, decimal balance, string pinCode)
        {
            AccountNumber = accountNumber;
            HolderName = holderName;
            Balance = balance;
            PinCode = pinCode;

        }
        public void Deposit (decimal amount)
        {
            Balance += amount;
            TransactionHistory.Add($"Deposit: {amount:C}");
        }
        public void Withdraw (decimal amount)
        {
            if(amount > Balance)
            {
                throw new InvalidOperationException("Insufficent balance");
            }
            Balance -= amount;
            TransactionHistory.Add($"Whithdrew: {amount:C}");
        }
    }
}
