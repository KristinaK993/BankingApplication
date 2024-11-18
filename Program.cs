using BankingApplication;
using Figgle;
using Spectre.Console;
class Program
{
    private static Repository<BankAccount> bankRepo = new("data.json");
    static void Main(string[] args)
    {
        AnsiConsole.MarkupLine($"[bold green]{FiggleFonts.Slant.Render("Kristinas Bank")}[/]");
        bankRepo.LoadData();

        while (true)
        {
            ShowLogIn();
        }
    }
    private static void ShowLogIn()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold blue] Welcome To Kristinas BankApp[/]");
        Console.Write("Enter account number: ");
        string accountNumber = Console.ReadLine();

        Console.Write("Enter PIN Code: ");
        string pinCode = Console.ReadLine();

        var account = bankRepo.GetAccountByNumber(accountNumber);

        if (account != null && account.PinCode == pinCode)
        {
            ShowMainMenu(account);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Invalid account number or PIN. Please try again.[/]");
        }
    }
    private static void ShowMainMenu(BankAccount account)
    {
        while (true)
        {
            Console.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold green]Select an option:[/]")
                    .AddChoices("1. View Balance", "2. Deposit", "3. Withdraw", "4. Transaction History", "5. Transfer", "6. Logout"));

            switch (choice)
            {
                case "1. View Balance":
                    AnsiConsole.MarkupLine($"[yellow]Your balance: {account.Balance:C}[/]");
                    break;

                case "2. Deposit":
                    PerformDeposit(account);
                    break;

                case "3. Withdraw":
                    PerformWithdraw(account);
                    break;

                case "4. Transaction History":
                    ShowTransactionHistory(account);
                    break;

                case "5. Transfer":
                    PerformTransfer(account);
                    break;

                case "6. Logout":
                    return;

            }
            AnsiConsole.MarkupLine("[blue]Press any key to continue...[/]");
            Console.ReadKey();
        }

    }
    private static void PerformDeposit(BankAccount account)
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
            AnsiConsole.MarkupLine("[red]Invalid amount.[/]");
        }
    }
    private static void PerformWithdraw(BankAccount account)
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
            catch(Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Invalid amount.[/]");
        }
    }
    private static void ShowTransactionHistory(BankAccount account)
    {
        AnsiConsole.MarkupLine("[bold yellow]Transaction History:[/]");
        foreach (var transaction in account.TransactionHistory)
        {
            AnsiConsole.MarkupLine($"[blue]{transaction}[/]");
        }
    }
    private static void PerformTransfer(BankAccount account)
    {
        Console.Write("Enter recipient account number: ");
        string recipientNumber = Console.ReadLine();

        Console.Write("Enter amount to transfer: ");
        if(decimal.TryParse(Console.ReadLine(),out decimal amount))
        {
            var recipient = bankRepo.GetAccountByNumber(recipientNumber);
            if (recipient != null)
            {
                try
                {
                    account.Withdraw(amount);
                    recipient.Deposit(amount);
                    bankRepo.SaveData();
                    AnsiConsole.MarkupLine("[green]Transfer successful![/]");
                }
                catch(Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Invalid amount.[/]");
            }
        }
    }

}

