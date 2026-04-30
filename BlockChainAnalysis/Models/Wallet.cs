namespace BlockChainAnalysis.Models
{
    public class Wallet
    {
        public string WalletId { get; set; }
        public decimal Balance { get; set; }

        public Wallet(string walletId)
        {
            WalletId = walletId;
            Balance = 0; // Başlangıç bakiyesi hesaplamalarla güncellenecek
        }
    }
}
