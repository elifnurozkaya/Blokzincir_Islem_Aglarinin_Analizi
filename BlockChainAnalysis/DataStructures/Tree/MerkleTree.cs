using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using BlockChainAnalysis.Models;

namespace BlockChainAnalysis.DataStructures.Tree
{
    // Düğüm (Node) Sınıfı - Ağacın her bir dalını veya yaprağını temsil eder.
    public class MerkleNode
    {
        public string Hash { get; set; }
        public MerkleNode LeftNode { get; set; }
        public MerkleNode RightNode { get; set; }

        public MerkleNode(string hash)
        {
            Hash = hash;
            LeftNode = null;
            RightNode = null;
        }
    }

    // Author: Abdullah Eren Korkmaz
    // Açıklama: Proje gereklilikleri kapsamında Merkle Ağacı veri yapısı tarafımca kodlanmıştır.
    public class MerkleTree
    {
        public MerkleNode Root { get; private set; }

        public MerkleTree()
        {
            Root = null;
        }

        // C#'ın gömülü kütüphanesi ile metinleri şifreleyen (Hashleyen) metod
        private string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // İşlem verisini benzersiz bir metne çeviren yardımcı metod
        private string GetTransactionString(Transaction tx)
        {
            // TransactionId zaten unique olabilir ama verilerin değiştirilip değiştirilmediğini
            // anlamak için tüm önemli alanları birleştirerek şifreliyoruz.
            return $"{tx.TransactionId}-{tx.FromWalletId}-{tx.ToWalletId}-{tx.Amount}-{tx.Timestamp.Ticks}";
        }

        // Verilen işlemler listesinden Merkle Ağacını oluşturur
        public void BuildTree(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                Root = null;
                return;
            }

            // 1. Adım: Tüm işlemleri hashleyip en alt yaprakları (Leaf Nodes) oluştur.
            List<MerkleNode> currentLayer = new List<MerkleNode>();
            foreach (var tx in transactions)
            {
                string txString = GetTransactionString(tx);
                string hash = ComputeHash(txString);
                currentLayer.Add(new MerkleNode(hash));
            }

            // 2. Adım: Ağacı aşağıdan yukarıya doğru (ikili ikili birleştirerek) inşa et.
            while (currentLayer.Count > 1)
            {
                List<MerkleNode> nextLayer = new List<MerkleNode>();

                // Çift çift dolaşmak için i'yi 2 artırıyoruz
                for (int i = 0; i < currentLayer.Count; i += 2)
                {
                    MerkleNode left = currentLayer[i];
                    
                    // Eğer sağdaki eleman yoksa (toplam sayı tek ise), solu kopyala (standart Merkle mantığı)
                    MerkleNode right = (i + 1 < currentLayer.Count) ? currentLayer[i + 1] : currentLayer[i];

                    // Sol ve Sağ child'ın hash'lerini birleştirip yeni bir parent hash oluştur.
                    string combinedHash = ComputeHash(left.Hash + right.Hash);
                    
                    MerkleNode parent = new MerkleNode(combinedHash)
                    {
                        LeftNode = left,
                        RightNode = right
                    };

                    nextLayer.Add(parent);
                }

                currentLayer = nextLayer;
            }

            // Döngü bittiğinde listede tek bir node kalır, o da Root'tur.
            Root = currentLayer[0];
        }

        // Merkle Root (Kök) değerini döndürür
        public string GetRootHash()
        {
            return Root?.Hash;
        }

        // Bütünlük Kontrolü: Verilen işlem listesiyle elimizdeki Root eşleşiyor mu?
        // Bu sayede RAM'deki Transaction listesinde sonradan bir değişiklik yapılmış mı anlarız.
        public bool VerifyIntegrity(List<Transaction> currentTransactions)
        {
            if (Root == null && (currentTransactions == null || currentTransactions.Count == 0))
                return true;

            if (Root == null || currentTransactions == null || currentTransactions.Count == 0)
                return false;

            // Mevcut durumu bozmamak için geçici bir ağaç oluşturup kökünü hesaplayalım
            MerkleTree tempTree = new MerkleTree();
            tempTree.BuildTree(currentTransactions);

            // Eğer geçici olarak hesaplanan kök, bizim orijinal kökümüzle aynıysa veriler güvendedir.
            return this.Root.Hash == tempTree.GetRootHash();
        }
    }
}
