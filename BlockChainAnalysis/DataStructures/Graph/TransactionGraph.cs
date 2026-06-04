using System;
using System.Collections.Generic;
using System.Linq;
using BlockChainAnalysis.Models;
using BlockChainAnalysis.DataStructures.HashTable; 

namespace BlockChainAnalysis.DataStructures.Graph
{
    // Author: Elifnur Özkaya
    // Açıklama: Proje gereklilikleri kapsamında Graf veri yapısı tarafımca kodlanmıştır.
    public class TransactionGraph
    {
       
        private WalletHashTable _vertices;

        // Komşuluk listesi (İşlemleri listelemek için standart Dictionary)
        private Dictionary<string, List<Transaction>> _adjacencyList;

        public TransactionGraph()
        {
            _vertices = new WalletHashTable(); // Kendi Hash Table nesnemiz başlatıldı
            _adjacencyList = new Dictionary<string, List<Transaction>>();
        }

        // Düğüm (Cüzdan) ekler
        public void AddVertex(Wallet wallet)
        {
           
            if (_vertices.Search(wallet.WalletId) == null)
            {
                _vertices.Insert(wallet.WalletId, wallet);
                _adjacencyList[wallet.WalletId] = new List<Transaction>();
            }
        }

        // Yönlü Kenar (İşlem/Transfer) ekler
        public void AddEdge(Transaction transaction)
        {
            if (!_adjacencyList.ContainsKey(transaction.FromWalletId))
            {
                AddVertex(new Wallet(transaction.FromWalletId));
            }

            if (!_adjacencyList.ContainsKey(transaction.ToWalletId))
            {
                AddVertex(new Wallet(transaction.ToWalletId));
            }

            _adjacencyList[transaction.FromWalletId].Add(transaction);
        }

        // Düğüm (Cüzdan) siler ve ona bağlı tüm işlemleri temizler
        public void RemoveVertex(string walletId)
        {
            
            if (_vertices.Search(walletId) != null)
            {
                _vertices.Delete(walletId);
            }

            if (_adjacencyList.ContainsKey(walletId))
            {
                _adjacencyList.Remove(walletId);
            }

            // Diğer cüzdanlardan bu cüzdana gelen veya giden tüm işlemleri temizle
            foreach (var key in _adjacencyList.Keys)
            {
                _adjacencyList[key].RemoveAll(tx => tx.ToWalletId == walletId || tx.FromWalletId == walletId);
            }
        }

        // ==================== ARAMA FONKSİYONLARI ====================

        public List<Transaction> BreadthFirstSearch(string startWalletId)
        {
            var result = new List<Transaction>();

            foreach (var key in _adjacencyList.Keys)
            {
                result.AddRange(_adjacencyList[key].Where(tx => tx.ToWalletId == startWalletId));
            }

            if (!_adjacencyList.ContainsKey(startWalletId))
                return result;

            var visited = new HashSet<string>(); 
            var queue = new Queue<string>();     

            visited.Add(startWalletId);
            queue.Enqueue(startWalletId);

            while (queue.Count > 0)
            {
                string currentWalletId = queue.Dequeue();

                foreach (var transaction in _adjacencyList[currentWalletId])
                {
                    result.Add(transaction);

                    if (!visited.Contains(transaction.ToWalletId))
                    {
                        visited.Add(transaction.ToWalletId);
                        queue.Enqueue(transaction.ToWalletId);
                    }
                }
            }

            return result;
        }

        public List<Transaction> DepthFirstSearch(string startWalletId)
        {
            var result = new List<Transaction>();

            foreach (var key in _adjacencyList.Keys)
            {
                result.AddRange(_adjacencyList[key].Where(tx => tx.ToWalletId == startWalletId));
            }

            if (!_adjacencyList.ContainsKey(startWalletId))
                return result;

            var visited = new HashSet<string>();
            var stack = new Stack<string>();     

            stack.Push(startWalletId);

            while (stack.Count > 0)
            {
                string currentWalletId = stack.Pop();

                if (visited.Contains(currentWalletId))
                    continue;

                visited.Add(currentWalletId);

                foreach (var transaction in _adjacencyList[currentWalletId])
                {
                    result.Add(transaction);

                    if (!visited.Contains(transaction.ToWalletId))
                    {
                        stack.Push(transaction.ToWalletId);
                    }
                }
            }

            return result;
        }

        public List<Transaction> GetNeighbors(string walletId)
        {
            if (_adjacencyList.ContainsKey(walletId))
                return _adjacencyList[walletId];

            return new List<Transaction>();
        }

        
        // veriler komşuluk listesi üzerinden Search edilerek toplandı.
        public IEnumerable<Wallet> GetAllWallets()
        {
            var allWallets = new List<Wallet>();
            foreach (var key in _adjacencyList.Keys)
            {
                var wallet = _vertices.Search(key);
                if (wallet != null) 
                {
                    allWallets.Add(wallet);
                }
            }
            return allWallets;
        }
    }
}
