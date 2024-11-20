using Spectre.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var userRepo = new Repository<BankUser>("accounts.json");

        // Visa en välkomstlogga och logga in
        ShowWelcomeMessage();

        BankAccount currentAccount = null;

        // Huvudloop för inloggning
        while (true)
        {
            Console.Write("Enter your user name to login: ");
            string userName = Console.ReadLine();

            if (Login(userRepo, userName, out currentAccount))
            {
                ShowAccountMenu(currentAccount, userRepo); // Skicka currentAccount här
                break; // Avsluta huvudloopen efter att kontomenyn har avslutats
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Login failed. Try again.[/]");
            }
        }
    }

    // Välkomstmeddelande
    public static void ShowWelcomeMessage()
    {
        AnsiConsole.MarkupLine("[bold green]Welcome to the Bank Application![/]");
        AnsiConsole.MarkupLine("Please login to proceed.");
    }

    // Inloggning
    private static bool Login(Repository<BankUser> userRepo, string userName, out BankAccount selectedAccount)
    {
        selectedAccount = null;

        // Hämta användaren från repo
        var user = userRepo.GetAll().FirstOrDefault(u => u.UserName == userName);
        if (user == null)
        {
            AnsiConsole.MarkupLine("[red]User not found.[/]");
            return false;
        }

        if (user.Accounts.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]User has no accounts. Please create an account first.[/]");
            return false;
        }

        // Verifiera PIN-koden
        Console.Write("Enter your PIN: ");
        string enteredPin = Console.ReadLine();
        if (!user.ValidatePin(enteredPin))
        {
            AnsiConsole.MarkupLine("[red]Invalid PIN.[/]");
            return false;
        }

        // Visa konton för att välja
        AnsiConsole.MarkupLine("[green]Login successful![/]");
        Console.WriteLine("Select an account:");

        for (int i = 0; i < user.Accounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {user.Accounts[i]}");
        }

        Console.Write("Enter the number of the account you want to access: ");
        if (int.TryParse(Console.ReadLine(), out int accountChoice) &&
            accountChoice > 0 && accountChoice <= user.Accounts.Count)
        {
            selectedAccount = user.Accounts[accountChoice - 1];
            return true;
        }

        AnsiConsole.MarkupLine("[red]Invalid account selection.[/]");
        return false;
    }

    // Konto-menyn
    private static void ShowAccountMenu(BankAccount account, Repository<BankUser> userRepo)
    {
        if (account == null)
        {
            AnsiConsole.MarkupLine("[red]Account is not found! Exiting...[/]");
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold green]Welcome, {account.AccountNumber}![/]");
            AnsiConsole.MarkupLine("[yellow]Select an option:[/]");
            AnsiConsole.MarkupLine("1. View Balance");
            AnsiConsole.MarkupLine("2. Deposit Money");
            AnsiConsole.MarkupLine("3. Withdraw Money");
            AnsiConsole.MarkupLine("4. View Transaction History");
            AnsiConsole.MarkupLine("5. Transfer Money To Another Account");
            AnsiConsole.MarkupLine("6. Save Changes");
            AnsiConsole.MarkupLine("7. Log Out");

            string choice = Console.ReadLine();

            // Hantera valet
            switch (choice)
            {
                case "1":
                    ViewBalance(account);
                    PauseBeforeReturning(); // Pausa innan återgång
                    break;
                case "2":
                    DepositMoney(account, userRepo);
                    PauseBeforeReturning();
                    break;
                case "3":
                    WithdrawMoney(account, userRepo);
                    PauseBeforeReturning();
                    break;
                case "4":
                    ViewTransactionHistory(account);
                    PauseBeforeReturning();
                    break;
                case "5":
                    TransferMoney(account, userRepo);
                    PauseBeforeReturning();
                    break;
                case "6":
                    SaveChanges(userRepo);
                    AnsiConsole.MarkupLine("[green]Changes saved successfully![/]");
                    PauseBeforeReturning();
                    break;
                case "7":
                    Console.Write("Do you want to save changes before logging out? (yes/no): ");
                    string saveChoice = Console.ReadLine()?.Trim().ToLower();
                    if (saveChoice == "yes" || saveChoice == "y")
                    {
                        SaveChanges(userRepo);
                        AnsiConsole.MarkupLine("[green]Changes saved successfully![/]");
                    }
                    AnsiConsole.MarkupLine("[green]You have logged out successfully, Bye![/]");
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Invalid choice! Please try again.[/]");
                    PauseBeforeReturning();
                    break;
            }
        }
    }

    // Pausfunktion för att ge användaren tid att läsa resultatet
    private static void PauseBeforeReturning()
    {
        AnsiConsole.MarkupLine("[grey]Press any key to return to the menu...[/]");
        Console.ReadKey(true); // Väntar på valfri tangent utan att visa den i konsolen
    }



    // Visa saldo
    private static void ViewBalance(BankAccount account)
    {
        AnsiConsole.MarkupLine($"[bold yellow]Account Balance: {account.Balance:C}[/]");
    }

    // Insättning
    private static void DepositMoney(BankAccount account, Repository<BankUser> userRepo)
    {
        while (true)
        {
            Console.Write("Enter amount to deposit: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                try
                {
                    account.Deposit(amount);
                    userRepo.SaveData();
                    AnsiConsole.MarkupLine("[green]Deposit successful![/]");
                    break; // Avsluta loopen efter en lyckad insättning
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid input! Please enter a valid amount.[/]");
            }
        }
    }


    // Uttag
    private static void WithdrawMoney(BankAccount account, Repository<BankUser> userRepo)
    {
        Console.Write("Enter amount to withdraw: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            try
            {
                // göra ett uttag
                account.Withdraw(amount);

                // Logga transaktionen
                account.TransactionHistory.Add($"Withdrew {amount:C} on {DateTime.Now}");

                // Spara ändringarna i JSON-filen
                userRepo.SaveData();

                AnsiConsole.MarkupLine("[green]Withdrawal successful![/]");
            }
            catch (Exception ex)
            {
                // Hantera eventuella fel, t.ex. otillräckligt saldo
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            }
        }
        else
        {
            // Hantera ogiltig inmatning
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
    private static void TransferMoney(BankAccount account, Repository<BankUser> userRepo)
    {
        Console.Write("Enter recipient account number: ");
        string recipientAccountNumber = Console.ReadLine();

        // Hämta alla användare från repositoriet
        var allUsers = userRepo.GetAll();

        // Leta efter mottagarkontot
        var recipientAccount = allUsers
            .SelectMany(user => user.Accounts)
            .FirstOrDefault(acc => acc.AccountNumber == recipientAccountNumber);

        if (recipientAccount != null)
        {
            Console.Write("Enter amount to transfer: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                try
                {
                    // Gör uttag från avsändarens konto
                    account.Withdraw(amount);

                    // Sätt in pengar på mottagarens konto
                    recipientAccount.Deposit(amount);

                    // Logga transaktionerna
                    account.TransactionHistory.Add($"Transferred {amount:C} to {recipientAccount.AccountNumber} on {DateTime.Now}");
                    recipientAccount.TransactionHistory.Add($"Received {amount:C} from {account.AccountNumber} on {DateTime.Now}");

                    // Spara ändringarna i JSON-filen
                    userRepo.SaveData();

                    AnsiConsole.MarkupLine("[green]Transfer successful![/]");
                }
                catch (Exception ex)
                {
                    // Hantera eventuella fel, t.ex. otillräckligt saldo
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid amount! Please try again.[/]");// Hantera ogiltig inmatning
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Recipient account not found![/]");
        }
    }


    // Skapa konto
    private static void CreateAccount(Repository<BankUser> userRepo)
    {
        Console.Write("Enter account holder name: ");
        string userName = Console.ReadLine();

        // Hämta användaren från userRepo
        var user = userRepo.GetAll().FirstOrDefault(u => u.UserName == userName);
        if (user == null)
        {
            AnsiConsole.MarkupLine("[red]User not found![/]");
            return;
        }

        Console.Write("Enter initial balance: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
        {
            AnsiConsole.MarkupLine("[red]Invalid initial balance![/]");
            return;
        }

        Console.Write("Enter a 4-digit PIN code: ");
        string pinCode = Console.ReadLine();
        if (pinCode.Length != 4 || !int.TryParse(pinCode, out _))
        {
            AnsiConsole.MarkupLine("[red]Invalid PIN code! Please enter a 4-digit number.[/]");
            return;
        }

        Console.Write("Enter account type (e.g., Savings or Personal): ");
        string accountType = Console.ReadLine();

        // Skapa ett unikt kontonummer
        string accountNumber = Guid.NewGuid().ToString().Substring(0, 8); // Unikt kontonummer

        // Skapa det nya kontot
        var newAccount = new BankAccount(accountNumber, accountType, initialBalance, pinCode);

        // Lägg till kontot till användaren
        user.AddAccount(newAccount);

        // Spara ändringarna
        userRepo.SaveData();

        AnsiConsole.MarkupLine($"[green]Account created successfully! Account Number: {newAccount.AccountNumber}[/]");
    }
    private static void SaveChanges(Repository<BankUser> userRepo)
    {
        try
        {
            userRepo.SaveData();
            AnsiConsole.MarkupLine("[green]Changes have been saved successfully![/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to save changes: {ex.Message}[/]");
        }
    }


}
