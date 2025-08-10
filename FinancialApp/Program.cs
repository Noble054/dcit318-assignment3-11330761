using System;
using System.Transactions;

record Transaction(int Id, DateTime Date, decimal Amount, string Category);

interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

class BankTransactionProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer: {transaction.Id}, {transaction.Date}, {transaction.Amount}, {transaction.Category}");
        Console.WriteLine("Transaction processed successfully via Bank Transfer.");
    }
}

class MobileMoneyTransactionProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Momo On the way : {transaction.Id}, {transaction.Date}, {transaction.Amount}, {transaction.Category}");
        Console.WriteLine("Transaction processed successfully via Mobile Money.");
    }
}

class CryptoTransactionProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing crypto transfer: {transaction.Id}, {transaction.Date}, {transaction.Amount}, {transaction.Category}");
        Console.WriteLine("Transaction processed successfully via Crypto.");
    }
}

class Account
{
    public String AccountNumber { get; set; }
    public Decimal Balance {get; protected set;}

    public Account(String AccountNumber, decimal initialBalance)
    {
        this.AccountNumber = AccountNumber;
        this.Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction) 
    { 
    Balance -= transaction.Amount;

    }
}

sealed class SavingsAccount : Account
{
    public SavingsAccount(String AccountNumber, decimal initialBalance) : base(AccountNumber, initialBalance)
    {
        if (initialBalance < 0)
        {
            throw new ArgumentException("Initial balance cannot be negative");
        }
    }
    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance) 
        {
            Console.WriteLine("Insufficient funds");

        }
        else 
        {
        Balance -= transaction.Amount;
        Console.WriteLine($"Transaction applied to Savings Account {AccountNumber}. New balance: {Balance}");

        }
    }
}


class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        
        var account = new SavingsAccount("Ha7927363jk", 50000M);

        
        var t1 = new Transaction(1, DateTime.Now, 150m, "Food stuff");
        var t2 = new Transaction(2, DateTime.Now, 200m, "Electricity");
        var t3 = new Transaction(3, DateTime.Now, 300m, "Savings");

     
        var mobileMoneyProcessor = new MobileMoneyTransactionProcessor();
        var bankProcessor = new BankTransactionProcessor();
        var cryptoProcessor = new CryptoTransactionProcessor();

        mobileMoneyProcessor.Process(t1);
        bankProcessor.Process(t2);
        cryptoProcessor.Process(t3);

        
        account.ApplyTransaction(t1);
        account.ApplyTransaction(t2);
        account.ApplyTransaction(t3);

       
        _transactions.Add(t1);
        _transactions.Add(t2);
        _transactions.Add(t3);
    }
}


class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();

        Console.WriteLine("All transactions processed successfully.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
