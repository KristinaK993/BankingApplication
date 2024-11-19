using System;
using Spectre.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var bankRepo = new Repository<BankAccount>(@"C:\Path\To\data.json");
        string filePath = @"C:\Path\To\data.json";

        // Visa en välkomstlogga och logga in
        ShowWelcomeMessage();
        var loggedIn = false;
        BankAccount currentAccount = null;

        while (!loggedIn)
        {
            Console.Write("Enter your account number to login: ");
            string accountNumber = Console.ReadLine();

            loggedIn = Login(bankRepo, accountNumber, out currentAccount);

            if (!loggedIn)
            {
                AnsiConsole.MarkupLine("[red]Login failed. Try again.[/]");
            }
        }

        // Om inloggningen lyckades, visa konto-menyn
        ShowAccountMenu(currentAccount, bankRepo);
    }

    // Visa välkomstlogga
    private static void ShowWelcomeMessage()
    {
        AnsiConsole.MarkupLine("[bold green]Welcome to Kristina's Bank App![/]");
        AnsiConsole.MarkupLine("[yellow]Please log in with your account number and PIN code.[/]");
    }

    // Login-funktion
    private static bool Login(Repository<BankAccount> bankRepo, string accountNumber, out BankAccount account)
    {
        account = bankRepo.GetAccountByNumber(accountNumber);
        if (account == null)
        {
            return false;  // Konto hittades inte
        }

        Console.Write("Enter your PIN code: ");
        string enteredPin = Console.ReadLine();

        // Validera PIN-koden
        return account.ValidatePin(enteredPin);
    }

    // Konto-menyn
    private static void ShowAccountMenu(BankAccount account, Repository<BankAccount> bankRepo)
    {
        bool continueUsingAccount = true;

        while (continueUsingAccount)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold green]Welcome, {account.UserName}![/]");
            AnsiConsole.MarkupLine("[yellow]Select an option:[/]");
            AnsiConsole.MarkupLine("1. View Balance");
            AnsiConsole.MarkupLine("2. Deposit Money");
            AnsiConsole.MarkupLine("3. Withdraw Money");
            AnsiConsole.MarkupLine("4. View Transaction History");
            AnsiConsole.MarkupLine("5. Transfer to Another Account");
            AnsiConsole.MarkupLine("6. Log Out");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ViewBalance(account);
                    break;
                case "2":
                    DepositMoney(account, bankRepo);
                    break;
                case "3":
                    WithdrawMoney(account, bankRepo);
                    break;
                case "4":
                    ViewTransactionHistory(account);
                    break;
                case "5":
                    TransferMoney(account, bankRepo);
                    break;
                case "6":
                    continueUsingAccount = false;
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Invalid choice! Try again.[/]");
                    break;
            }
        }

        // Logga ut
        AnsiConsole.MarkupLine("[green]You have logged out successfully.[/]");
    }

    // Visa saldo
    private static void ViewBalance(BankAccount account)
    {
        AnsiConsole.MarkupLine($"[bold yellow]Account Balance: {account.Balance:C}[/]");
    }

    // Insättning
    private static void DepositMoney(BankAccount account, Repository<BankAccount> bankRepo)
    {
        Console.Write("Enter amount to deposit: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            account.Deposit(amount);
            bankRepo.SaveData();
            AnsiConsole.MarkupLine("[green]Deposit successful![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Invalid amount! Please try again.[/]");
        }
    }

    // Uttag
    private static void WithdrawMoney(BankAccount account, Repository<BankAccount> bankRepo)
    {
        Console.Write("Enter amount to withdraw: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            try
            {
                account.Withdraw(amount);
                bankRepo.SaveData();
                AnsiConsole.MarkupLine("[green]Withdrawal successful![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Invalid amount! Please try again.[/]");
        }
    }

    // Visa transaktionshistorik
    private static void ViewTransactionHistory(BankAccount account)
    {
        AnsiConsole.MarkupLine("[bold yellow]Transaction History:[/]");
        foreach (var transaction in account.TransactionHistory)
        {
            AnsiConsole.MarkupLine($"[blue]{transaction}[/]");
        }
    }

    // Överföring mellan konton
    private static void TransferMoney(BankAccount account, Repository<BankAccount> bankRepo)
    {
        Console.Write("Enter recipient account number: ");
        string recipientAccountNumber = Console.ReadLine();
        var recipientAccount = bankRepo.GetAccountByNumber(recipientAccountNumber);

        if (recipientAccount != null)
        {
            Console.Write("Enter amount to transfer: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                try
                {
                    account.Withdraw(amount);
                    recipientAccount.Deposit(amount);
                    bankRepo.SaveData();
                    AnsiConsole.MarkupLine("[green]Transfer successful![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid amount! Please try again.[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Recipient account not found![/]");
        }
    }

    // Skapa konto
    private static void CreateAccount(Repository<BankAccount> bankRepo)
    {
        Console.Write("Enter account holder name: ");
        string holderName = Console.ReadLine();

        Console.Write("Enter initial balance: ");
        decimal initialBalance;
        if (!decimal.TryParse(Console.ReadLine(), out initialBalance))
        {
            AnsiConsole.MarkupLine("[red]Invalid amount![/]");
            return;
        }

        Console.Write("Enter a 4-digit PIN code: ");
        string pinCode = Console.ReadLine();
        if (pinCode.Length != 4 || !int.TryParse(pinCode, out _))
        {
            AnsiConsole.MarkupLine("[red]Invalid PIN code! Please enter a 4-digit number.[/]");
            return;
        }

        string accountNumber = Guid.NewGuid().ToString().Substring(0, 8);  // Skapa ett unikt konto nummer
        var newAccount = new BankAccount(accountNumber, holderName, initialBalance, pinCode);

        // Lägg till kontot i repository
        bankRepo.Add(newAccount);

        AnsiConsole.MarkupLine("[green]Account created and saved successfully![/]");
    }
}

