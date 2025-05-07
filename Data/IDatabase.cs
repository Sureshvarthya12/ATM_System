using System.Collections.Generic;
using System;

namespace ATMSystem.Data
{
    public interface IDatabase
    {
        User? GetUserByLogin(string login);
        Account? GetAccount(int accountNumber);
        bool CreateAccount(Customer customer, Account account);
        bool UpdateAccount(Account account);
        bool DeleteAccount(int accountNumber);
        int AddTransaction(Transaction transaction);
        List<Transaction> GetTransactions(int accountNumber);
    }
}
