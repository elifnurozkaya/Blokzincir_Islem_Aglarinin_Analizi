using System;
using BlockChainAnalysis.Models;

namespace BlockChainAnalysis.DataStructures.HashTable
{
    // 3. KİŞİ TARAFINDAN DOLDURULACAKTIR
    public class WalletHashTable
    {
        // TODO: Hash tablosu için dizi (array) ve collision (çarpışma) çözme yöntemini tanımlayın
        
        public WalletHashTable()
        {
            // TODO: Başlangıç kapasitesini ve diziyi ayarlayın
        }

        // Karma Fonksiyonu (Hash Function)
        private int HashFunction(string key)
        {
            throw new NotImplementedException();
        }

        // Cüzdan bilgisini Hash tablosuna ekler
        public void Insert(string walletId, Wallet walletData)
        {
            throw new NotImplementedException();
        }

        // Cüzdan ID'sine göre ortalama O(1) sürede cüzdanı bulur
        public Wallet Search(string walletId)
        {
            throw new NotImplementedException();
        }

        // Cüzdanı Hash tablosundan siler
        public void Delete(string walletId)
        {
            throw new NotImplementedException();
        }
    }
}
