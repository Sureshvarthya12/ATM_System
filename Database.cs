using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ATMSystem
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUserByLogin(string login)
        {
            User user = null;

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT u.*, ca.account_number FROM users u " +
                    "LEFT JOIN customer_accounts ca ON u.id = ca.user_id " +
                    "WHERE u.login = @login", connection);
                command.Parameters.AddWithValue("@login", login);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var userType = reader.GetString("user_type");
                    var id = reader.GetInt32("id");
                    var name = reader.GetString("name");
                    var pinCode = reader.GetString("pin_code");

                    if (userType == "Customer")
                    {
                        var customer = new Customer
                        {
                            Id = id,
                            Login = login,
                            PinCode = pinCode,
                            Name = name
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("account_number")))
                            customer.AccountNumber = reader.GetInt32("account_number");

                        user = customer;
                    }
                    else if (userType == "Administrator")
                    {
                        user = new Administrator
                        {
                            Id = id,
                            Login = login,
                            PinCode = pinCode,
                            Name = name
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return user;
        }

        public Account GetAccount(int accountNumber)
        {
            Account account = null;

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM accounts WHERE account_number = @accountNumber", connection);
                command.Parameters.AddWithValue("@accountNumber", accountNumber);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    account = new Account
                    {
                        AccountNumber = reader.GetInt32("account_number"),
                        HolderName = reader.GetString("holder_name"),
                        Balance = reader.GetDecimal("balance"),
                        Status = Enum.Parse<AccountStatus>(reader.GetString("status"))
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return account;
        }

        public List<Account> GetAllAccounts()
        {
            var accounts = new List<Account>();

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM accounts", connection);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    accounts.Add(new Account
                    {
                        AccountNumber = reader.GetInt32("account_number"),
                        HolderName = reader.GetString("holder_name"),
                        Balance = reader.GetDecimal("balance"),
                        Status = Enum.Parse<AccountStatus>(reader.GetString("status"))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return accounts;
        }

        public bool CreateAccount(Customer customer, Account account)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();
                try
                {
                    var accountCommand = new MySqlCommand(
                        "INSERT INTO accounts (holder_name, balance, status) " +
                        "VALUES (@holderName, @balance, @status); SELECT LAST_INSERT_ID();",
                        connection, transaction);
                    accountCommand.Parameters.AddWithValue("@holderName", account.HolderName);
                    accountCommand.Parameters.AddWithValue("@balance", account.Balance);
                    accountCommand.Parameters.AddWithValue("@status", account.Status.ToString());

                    account.AccountNumber = Convert.ToInt32(accountCommand.ExecuteScalar());

                    var userCommand = new MySqlCommand(
                        "INSERT INTO users (login, pin_code, name, user_type) " +
                        "VALUES (@login, @pinCode, @name, 'Customer'); SELECT LAST_INSERT_ID();",
                        connection, transaction);
                    userCommand.Parameters.AddWithValue("@login", customer.Login);
                    userCommand.Parameters.AddWithValue("@pinCode", customer.PinCode);
                    userCommand.Parameters.AddWithValue("@name", customer.Name);

                    customer.Id = Convert.ToInt32(userCommand.ExecuteScalar());
                    customer.AccountNumber = account.AccountNumber;

                    var linkCommand = new MySqlCommand(
                        "INSERT INTO customer_accounts (user_id, account_number) " +
                        "VALUES (@userId, @accountNumber)", connection, transaction);
                    linkCommand.Parameters.AddWithValue("@userId", customer.Id);
                    linkCommand.Parameters.AddWithValue("@accountNumber", account.AccountNumber);
                    linkCommand.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Transaction error: {ex.Message}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        public bool UpdateAccount(Account account)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE accounts SET holder_name = @holderName, balance = @balance, status = @status " +
                    "WHERE account_number = @accountNumber", connection);
                command.Parameters.AddWithValue("@holderName", account.HolderName);
                command.Parameters.AddWithValue("@balance", account.Balance);
                command.Parameters.AddWithValue("@status", account.Status.ToString());
                command.Parameters.AddWithValue("@accountNumber", account.AccountNumber);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        public bool DeleteAccount(int accountNumber)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand(
                    "DELETE FROM accounts WHERE account_number = @accountNumber", connection);
                command.Parameters.AddWithValue("@accountNumber", accountNumber);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        public int AddTransaction(Transaction transaction)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO transactions (account_number, transaction_type, amount, balance_after) " +
                    "VALUES (@accountNumber, @type, @amount, @balanceAfter); SELECT LAST_INSERT_ID();",
                    connection);
                command.Parameters.AddWithValue("@accountNumber", transaction.AccountNumber);
                command.Parameters.AddWithValue("@type", transaction.Type.ToString());
                command.Parameters.AddWithValue("@amount", transaction.Amount);
                command.Parameters.AddWithValue("@balanceAfter", transaction.BalanceAfter);

                transaction.Id = Convert.ToInt32(command.ExecuteScalar());
                transaction.Date = DateTime.Now;
                return transaction.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return -1;
            }
        }

        public List<Transaction> GetTransactions(int accountNumber)
        {
            var transactions = new List<Transaction>();

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT * FROM transactions WHERE account_number = @accountNumber ORDER BY transaction_date DESC",
                    connection);
                command.Parameters.AddWithValue("@accountNumber", accountNumber);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    transactions.Add(new Transaction
                    {
                        Id = reader.GetInt32("id"),
                        AccountNumber = reader.GetInt32("account_number"),
                        Type = Enum.Parse<TransactionType>(reader.GetString("transaction_type")),
                        Amount = reader.GetDecimal("amount"),
                        Date = reader.GetDateTime("transaction_date"),
                        BalanceAfter = reader.GetDecimal("balance_after")
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            return transactions;
        }
    }
}
