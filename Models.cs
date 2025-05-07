using System;
using System.Collections.Generic;

namespace ATMSystem
{
    public abstract class User
    {
        public int Id { get; set; }
        public required string Login { get; set; }
        public required string PinCode { get; set; }
        public required string Name { get; set; }
        public UserType Type { get; set; }

        public bool ValidatePin(string enteredPin) => PinCode == enteredPin;
    }

    public class Customer : User
    {
        public int AccountNumber { get; set; }

        public Customer()
        {
            Type = UserType.Customer;
        }
    }

    public class Administrator : User
    {
        public Administrator()
        {
            Type = UserType.Administrator;
        }
    }

    public class Account
    {
        public int AccountNumber { get; set; }
        public required string HolderName { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }

        public bool Withdraw(decimal amount)
        {
            if (Status != AccountStatus.Active || amount <= 0 || amount > Balance)
                return false;

            Balance -= amount;
            return true;
        }

        public bool Deposit(decimal amount)
        {
            if (Status != AccountStatus.Active || amount <= 0)
                return false;

            Balance += amount;
            return true;
        }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public decimal BalanceAfter { get; set; }
    }

    public enum UserType
    {
        Customer,
        Administrator
    }

    public enum AccountStatus
    {
        Active,
        Disabled,
        Closed
    }

    public enum TransactionType
    {
        Withdrawal,
        Deposit
    }
}
