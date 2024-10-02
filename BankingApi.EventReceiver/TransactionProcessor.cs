using BankingApi.EventReceiver.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApi.EventReceiver
{
    public class TransactionProcessor : ITransactionProcessor
    {
        public void ProcessTransaction(Transaction transaction)
        {
            var bankingApiDbContext = new BankingApiDbContext();

            ValidateTransaction(transaction, bankingApiDbContext);

            if (transaction.MessageType == "Credit")
            {
                var account = bankingApiDbContext.BankAccounts.First(bankAccount => bankAccount.Id == transaction.BankAccountId);
                account.Balance += transaction.Amount;
            }
            else if (transaction.MessageType == "Debit")
            {
                var account = bankingApiDbContext.BankAccounts.First(bankAccount => bankAccount.Id == transaction.BankAccountId);
                account.Balance -= transaction.Amount;
            }
            else
            {
                throw new BusinessLogicException($"Unable to process this message type: {transaction.MessageType}");
            }
        }

        private void ValidateTransaction(Transaction transaction, BankingApiDbContext bankingApiDbContext)
        {
            if(transaction == null) { throw new ArgumentNullException(nameof(Transaction)); };

            if (string.IsNullOrWhiteSpace(transaction.MessageType))
                throw new ArgumentException("Message type is invalid");

            if(!bankingApiDbContext.BankAccounts.Any(account => account.Id == transaction.BankAccountId))
            {
                throw new BusinessLogicException("Account record not found");
            }    
            if(transaction.Amount <=0)
            {
                throw new BusinessLogicException("Invalid amount");
            }
        }
    }
}
