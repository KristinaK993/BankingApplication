Banking Application
A comprehensive, console-based banking application built in C# that allows users to manage their accounts, perform transactions, and track their financial history. The application ensures robust functionality through secure data management and an interactive command-line interface.

Features
1. User Management
Login System: Login with username and PIN for secure access.
Account Creation: Create accounts with an initial deposit, PIN code, and account type.
Multi-Account Support: Users can own and manage multiple accounts.
2. Banking Operations
View Balance: Check the current balance of your account.
Deposit Money: Add funds to your account with input validation.
Withdraw Money: Withdraw funds, ensuring sufficient balance is maintained.
Transfer Money: Transfer funds between accounts securely.
Transaction History: View a detailed log of all transactions associated with an account.
3. Data Persistence
JSON-based Storage: User and account data is stored in accounts.json, ensuring persistence across sessions.
Secure PIN Handling: PIN codes are validated during login and account creation.
Getting Started
Prerequisites
.NET 8.0
Running the Application
Clone the repository:https://github.com/KristinaK993/BankingApplication.git

bash
Kopiera kod
git clone https://github.com/KristinaK993/banking-application.git
cd banking-application
Build and run the project:

bash
Kopiera kod
dotnet run
Follow the on-screen instructions to login, register accounts, or perform banking operations.

Key Functionalities
Login and Account Selection
Login using a valid username and PIN.
Users are prompted to select an account from their registered list.
Menu Options
View Balance:

Displays the current balance in the account.
Example output:
bash
Kopiera kod
Account Balance: $500.00
Deposit Money:

Enter an amount to deposit into the account.
Validates for positive values.
Withdraw Money:

Withdraw funds, ensuring the account maintains a sufficient balance.
Logs the withdrawal in the transaction history.
View Transaction History:

Displays all past transactions with timestamps.
Example output:
yaml
Kopiera kod
Transaction History:
- Deposited $100 on 2024-11-25
- Withdrew $50 on 2024-11-26
Transfer Money:

Transfer funds to another account by specifying the recipient's account number.
Validates sufficient balance before transfer.
Save Changes:

Ensures all data changes are saved to accounts.json.
Logout:

Optionally save changes before logging out.
Validations and Security
Username Validation: Ensures username exists during login.
PIN Validation: PIN must be a 4-digit numeric value.
Transaction Amount Validation: Amounts must be positive values.
Account Validation: Transfers require valid recipient accounts.
Project Structure
Main Components
Program.cs:

Entry point for the application.
Handles login, menu navigation, and account actions.
BankAccount Class:

Represents a single account with properties like balance, transaction history, and account type.
Contains methods for deposit, withdrawal, and transfer.
BankUser Class:

Represents a user with a collection of bank accounts and methods for PIN validation.
Repository<T> Class:

Generic repository for data management.
Reads and writes data to accounts.json.
Sample File: accounts.json
json
Kopiera kod

Data Storage
The application uses a JSON file (accounts.json) to store user and account information persistently. The structure includes:

Users with a username and PIN.
Each user can have multiple accounts with properties such as account number, type, balance, and transaction history.

Known Limitations
PIN Security: PINs are stored in plaintext in the JSON file. Consider hashing for better security.
No Interest Calculations: No automatic interest accrual for savings accounts.
Multi-Session Transfers: Recipient accounts must exist in the same session.
Future Enhancements
Encrypted PIN Handling: Secure PINs using hashing algorithms.
GUI Implementation: Develop a graphical interface for improved user experience.
Email Notifications: Notify users about transactions and account updates.
Advanced Account Types: Support for loans, credit cards, and fixed deposits.
License
This project is licensed under the MIT License. See LICENSE for more details.
