using System;
using System.Linq;
using System.Security.Principal;
using System.Transactions;

namespace ATMSystem
{
    public class ATM
    {
        private readonly Database _database;
        private User _currentUser;

        public ATM(Database database)
        {
            _database = database;
        }

        public void Start()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the ATM System");
                Console.WriteLine("------------------------");

                // Login process
                if (!Login())
                {
                    Console.WriteLine("Would you like to try again? (y/n)");
                    if (Console.ReadLine().ToLower() != "y")
                        running = false;

                    continue;
                }

                // Route to appropriate menu based on user type
                if (_currentUser is Customer customer)
                {
                    RunCustomerMenu(customer);
                }
                else if (_currentUser is Administrator)
                {
                    RunAdministratorMenu();
                }

                // Log out
                _currentUser = null;

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }

            Console.WriteLine("Thank you for using the ATM System. Goodbye!");
        }

        private bool Login()
        {
            Console.Write("Enter login: ");
            Console.ForegroundColor = ConsoleColor.Green;
            string login = Console.ReadLine();
            Console.ResetColor();

            Console.Write("Enter Pin code: ");
            Console.ForegroundColor = ConsoleColor.Green;
            string pinCode = Console.ReadLine();
            Console.ResetColor();

            // Validate pin code format
            if (pinCode.Length != 5 || !pinCode.All(char.IsDigit))
            {
                Console.WriteLine("\nError: Pin Code must be a 5-digit number.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            var user = _database.GetUserByLogin(login);

            if (user == null || !user.ValidatePin(pinCode))
            {
                Console.WriteLine("\nInvalid login or pin code. Please try again.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return false;
            }

            _currentUser = user;
            return true;
        }

        private void RunCustomerMenu(Customer customer)
        {
            var account = _database.GetAccount(customer.AccountNumber);

            if (account == null)
            {
                Console.WriteLine("Error: Account not found. Please contact an administrator.");
                return;
            }

            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine($"Welcome, {customer.Name}");
                Console.WriteLine("------------------------");
                Console.WriteLine("1----Withdraw Cash");
                Console.WriteLine("3----Deposit Cash");
                Console.WriteLine("4----Display Balance");
                Console.WriteLine("5----Exit");
                Console.Write("\nPlease select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        WithdrawCash(account);
                        break;
                    case "3":
                        DepositCash(account);
                        break;
                    case "4":
                        DisplayBalance(account);
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private void WithdrawCash(Account account)
        {
            Console.Write("Enter the withdrawal amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
                return;
            }

            if (account.Withdraw(amount))
            {
                // Update the account in the database
                _database.UpdateAccount(account);

                // Record the transaction
                var transaction = new Transaction
                {
                    AccountNumber = account.AccountNumber,
                    Type = TransactionType.Withdrawal,
                    Amount = amount,
                    BalanceAfter = account.Balance,
                    Date = DateTime.Now
                };

                _database.AddTransaction(transaction);

                Console.WriteLine("Cash Successfully Withdrawn");
                Console.WriteLine($"Account #{account.AccountNumber}");
                Console.WriteLine($"Date: {DateTime.Now:MM/dd/yyyy}");
                Console.WriteLine($"Withdrawn: {amount}");
                Console.WriteLine($"Balance: {account.Balance:N0}");
            }
            else
            {
                Console.WriteLine("Withdrawal failed. Please check your balance and try again.");
            }
        }

        private void DepositCash(Account account)
        {
            Console.Write("Enter the cash amount to deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount. Please enter a positive number.");
                return;
            }

            if (account.Deposit(amount))
            {
                // Update the account in the database
                _database.UpdateAccount(account);

                // Record the transaction
                var transaction = new Transaction
                {
                    AccountNumber = account.AccountNumber,
                    Type = TransactionType.Deposit,
                    Amount = amount,
                    BalanceAfter = account.Balance,
                    Date = DateTime.Now
                };

                _database.AddTransaction(transaction);

                Console.WriteLine("Cash Deposited Successfully.");
                Console.WriteLine($"Account #{account.AccountNumber}");
                Console.WriteLine($"Date: {DateTime.Now:MM/dd/yyyy}");
                Console.WriteLine($"Deposited: {amount}");
                Console.WriteLine($"Balance: {account.Balance:N0}");
            }
            else
            {
                Console.WriteLine("Deposit failed. Please try again.");
            }
        }

        private void DisplayBalance(Account account)
        {
            Console.WriteLine($"Account #{account.AccountNumber}");
            Console.WriteLine($"Date: {DateTime.Now:MM/dd/yyyy}");
            Console.WriteLine($"Balance: {account.Balance:N0}");
        }

        private void RunAdministratorMenu()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine($"Administrator Menu - {_currentUser.Name}");
                Console.WriteLine("------------------------");
                Console.WriteLine("1----Create New Account");
                Console.WriteLine("2----Delete Existing Account");
                Console.WriteLine("3----Update Account Information");
                Console.WriteLine("4----Search for Account");
                Console.WriteLine("6----Exit");
                Console.Write("\nPlease select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateNewAccount();
                        break;
                    case "2":
                        DeleteExistingAccount();
                        break;
                    case "3":
                        UpdateAccountInformation();
                        break;
                    case "4":
                        SearchForAccount();
                        break;
                    case "6":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        private void CreateNewAccount()
        {
            Console.WriteLine("Create New Account");
            Console.WriteLine("-----------------");

            // Get customer information
            Console.Write("Login: ");
            string login = Console.ReadLine();

            // Check if login already exists
            if (_database.GetUserByLogin(login) != null)
            {
                Console.WriteLine("Error: Login already exists. Please choose a different login.");
                return;
            }

            Console.Write("Pin Code: ");
            string pinCode = Console.ReadLine();

            // Validate pin code
            if (pinCode.Length != 5 || !pinCode.All(char.IsDigit))
            {
                Console.WriteLine("Error: Pin Code must be a 5-digit number.");
                return;
            }

            Console.Write("Holders Name: ");
            string holderName = Console.ReadLine();

            Console.Write("Starting Balance: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal balance) || balance < 0)
            {
                Console.WriteLine("Error: Starting Balance must be a non-negative number.");
                return;
            }

            Console.Write("Status (Active/Disabled): ");
            string statusInput = Console.ReadLine().ToLower();
            AccountStatus status = statusInput == "disabled" ? AccountStatus.Disabled : AccountStatus.Active;

            // Create the customer and account
            var customer = new Customer
            {
                Login = login,
                PinCode = pinCode,
                Name = holderName
            };

            var account = new Account
            {
                HolderName = holderName,
                Balance = balance,
                Status = status
            };

            if (_database.CreateAccount(customer, account))
            {
                Console.WriteLine($"Account Successfully Created -- the account number assigned is: {account.AccountNumber}");
            }
            else
            {
                Console.WriteLine("Error: Failed to create account. Please try again.");
            }
        }

        private void DeleteExistingAccount()
        {
            Console.WriteLine("Delete Existing Account");
            Console.WriteLine("----------------------");

            Console.Write("Enter the account number to which you want to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Error: Invalid account number.");
                return;
            }

            var account = _database.GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Error: Account not found.");
                return;
            }

            Console.WriteLine($"You wish to delete the account held by {account.HolderName}. If this information is correct, please re-enter the account number: ");
            if (!int.TryParse(Console.ReadLine(), out int confirmAccountNumber) || confirmAccountNumber != accountNumber)
            {
                Console.WriteLine("Account deletion cancelled.");
                return;
            }

            if (_database.DeleteAccount(accountNumber))
            {
                Console.WriteLine("Account Deleted Successfully");
            }
            else
            {
                Console.WriteLine("Error: Failed to delete account. Please try again.");
            }
        }

        private void UpdateAccountInformation()
        {
            Console.WriteLine("Update Account Information");
            Console.WriteLine("-------------------------");

            Console.Write("Enter the Account Number: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Error: Invalid account number.");
                return;
            }

            var account = _database.GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Error: Account not found.");
                return;
            }

            // Display current account information
            DisplayAccountInformation(account);

            // Update holder name
            Console.Write("New Holder Name (leave blank to keep current): ");
            string holderName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(holderName))
            {
                account.HolderName = holderName;
            }

            // Update status
            Console.Write("New Status (Active/Disabled, leave blank to keep current): ");
            string statusInput = Console.ReadLine().ToLower();
            if (!string.IsNullOrWhiteSpace(statusInput))
            {
                account.Status = statusInput == "disabled" ? AccountStatus.Disabled : AccountStatus.Active;
            }

            // Update the account in the database
            if (_database.UpdateAccount(account))
            {
                Console.WriteLine("Account Information Updated Successfully");
            }
            else
            {
                Console.WriteLine("Error: Failed to update account information. Please try again.");
            }
        }

        private void SearchForAccount()
        {
            Console.WriteLine("Search for Account");
            Console.WriteLine("-----------------");

            Console.Write("Enter Account number: ");
            if (!int.TryParse(Console.ReadLine(), out int accountNumber))
            {
                Console.WriteLine("Error: Invalid account number.");
                return;
            }

            var account = _database.GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine("Error: Account not found.");
                return;
            }

            Console.WriteLine("The account information is:");
            DisplayAccountInformation(account);
        }

        private void DisplayAccountInformation(Account account)
        {
            Console.WriteLine($"Account # {account.AccountNumber}");
            Console.WriteLine($"Holder: {account.HolderName}");
            Console.WriteLine($"Balance: {account.Balance:N0}");
            Console.WriteLine($"Status: {account.Status}");
        }
    }
}