using System;
using System.Collections.Generic;
using BlockChainAnalysis.Models;

namespace BlockChainAnalysis.DataStructures.Tree
{
    // 2. KİŞİ TARAFINDAN DOLDURULACAKTIR
    public class MerkleTree
    {
        // TODO: Ağaç yapısı için Node (Düğüm) sınıfını veya yapısını tanımlayın

        public MerkleTree()
        {
            // TODO: Başlangıç atamalarını yapın
        }

        // Verilen işlemler listesinden Merkle Ağacını oluşturur
        public void BuildTree(List<Transaction> transactions)
        {
            throw new NotImplementedException();
        }

        // Merkle Root (Kök) değerini döndürür
        public string GetRootHash()
        {
            throw new NotImplementedException();
        }

        // Bir işlemin (Transaction) ağaçta geçerli olup olmadığını doğrular
        public bool VerifyTransaction(string transactionId)
        {
            throw new NotImplementedException();
        }
    }
}
