using Spectre.Console;
using Figgle;

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
                ShowAccountMenu(currentAccount, userRepo);
                break; // Avsluta huvudloopen efter att kontomenyn har avslutats
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Login failed. Try again.[/]");
            }
        }
    }

    public static void ShowWelcomeMessage()
    {
        
        var figgleText = FiggleFonts.Slant.Render("Bank Application");

       
        AnsiConsole.Write(
            new Panel(figgleText)
                .Border(BoxBorder.Double)
                .BorderStyle(new Style(foreground: Color.Green))
                .Header("[bold yellow]Welcome[/]")
                .HeaderAlignment(Justify.Center)
                .Expand() // Expandera panelen så att den fyller hela terminalens bredd
        );

        AnsiConsole.Write(
            new Rule("[yellow]Please log in to proceed[/]")
                .RuleStyle(new Style(foreground: Color.Blue))
                .Centered()
        );
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
        var accountChoices = user.Accounts
            .Select((account, index) => $"{index + 1}. {account.AccountType} - {account.AccountNumber}")
            .ToList();

        var selectedAccountChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select an account:[/]")
                .PageSize(5)
                .AddChoices(accountChoices)
        );

        // Matcha valt konto
        int selectedIndex = accountChoices.IndexOf(selectedAccountChoice);
        selectedAccount = user.Accounts[selectedIndex];

        return true;
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
            AnsiConsole.Clear(); // Rensar konsolen för en ren vy
            AnsiConsole.MarkupLine($"[bold green]Welcome, {account.AccountNumber}![/]");
            AnsiConsole.MarkupLine($"[yellow]Account Type: {account.AccountType}[/]");
            AnsiConsole.MarkupLine($"[cyan]Balance: {account.Balance:C}[/]\n");

            // Meny med Spectre.Console
            var menuOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an option:[/]")
                    .PageSize(7)
                    .AddChoices(new[]
                    {
                        "View Balance",
                        "Deposit Money",
                        "Withdraw Money",
                        "View Transaction History",
                        "Transfer Money",
                        "Save Changes",
                        "Log Out"
                    })
            );

            // Hantera menyval
            switch (menuOption)
            {
                case "View Balance":
                    ViewBalance(account);
                    PauseBeforeReturning();
                    break;
                case "Deposit Money":
                    DepositMoney(account, userRepo);
                    PauseBeforeReturning();
                    break;
                case "Withdraw Money":
                    WithdrawMoney(account, userRepo);
                    PauseBeforeReturning();
                    break;
                case "View Transaction History":
                    ViewTransactionHistory(account);
                    PauseBeforeReturning();
                    break;
                case "Transfer Money":
                    TransferMoney(account, userRepo);
                    PauseBeforeReturning();
                    break;
                case "Save Changes":
                    SaveChanges(userRepo);
                    AnsiConsole.MarkupLine("[green]Changes saved successfully![/]");
                    PauseBeforeReturning();
                    break;
                case "Log Out":
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
                    break;
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
                account.Withdraw(amount);
                account.TransactionHistory.Add($"Withdrew {amount:C} on {DateTime.Now}");
                userRepo.SaveData();
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
    private static void TransferMoney(BankAccount account, Repository<BankUser> userRepo)
    {
        Console.Write("Enter recipient account number: ");
        string recipientAccountNumber = Console.ReadLine();

        var allUsers = userRepo.GetAll();
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
                    account.Withdraw(amount);
                    recipientAccount.Deposit(amount);
                    account.TransactionHistory.Add($"Transferred {amount:C} to {recipientAccount.AccountNumber} on {DateTime.Now}");
                    recipientAccount.TransactionHistory.Add($"Received {amount:C} from {account.AccountNumber} on {DateTime.Now}");
                    userRepo.SaveData();
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

    // Spara ändringar
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
