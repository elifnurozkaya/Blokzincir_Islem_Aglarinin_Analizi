using System;

namespace BlockChainAnalysis.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string FromWalletId { get; set; }
        public string ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }

        public Transaction(string transactionId, string fromWalletId, string toWalletId, decimal amount, DateTime timestamp)
        {
            TransactionId = transactionId;
            FromWalletId = fromWalletId;
            ToWalletId = toWalletId;
            Amount = amount;
            Timestamp = timestamp;
        }
    }
}
