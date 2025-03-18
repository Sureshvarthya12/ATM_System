using System;
using System.Collections.Generic;

namespace ATMSystem
{
    // User model with common properties
    public abstract class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PinCode { get; set; }
        public string Name { get; set; }
        public UserType Type { get; set; }

        public bool ValidatePin(string enteredPin)
        {
            return PinCode == enteredPin;
        }
    }

    // Customer extends User
    public class Customer : User
    {
        public int AccountNumber { get; set; }

        public Customer()
        {
            Type = UserType.Customer;
        }
    }

    // Administrator extends User
    public class Administrator : User
    {
        public Administrator()
        {
            Type = UserType.Administrator;
        }
    }

    // Account model
    public class Account
    {
        public int AccountNumber { get; set; }
        public string HolderName { get; set; }
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

    // Transaction model
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public decimal BalanceAfter { get; set; }
    }

    // Enums
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