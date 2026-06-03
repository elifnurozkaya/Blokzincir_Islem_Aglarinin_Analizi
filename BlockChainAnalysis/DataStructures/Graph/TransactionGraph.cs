using System;
using System.Collections.Generic;
using BlockChainAnalysis.Models;

namespace BlockChainAnalysis.DataStructures.Graph
{
    // Author: Elifnur Özkaya
    // Açıklama: Proje gereklilikleri kapsamında Graf veri yapısı tarafımca kodlanmıştır.
    public class TransactionGraph
    {
        // Cüzdanları tutan dictionary (WalletId -> Wallet)
        private Dictionary<string, Wallet> _vertices;

        // Komşuluk listesi (WalletId -> o cüzdandan çıkan işlemler)
        private Dictionary<string, List<Transaction>> _adjacencyList;

        public TransactionGraph()
        {
            _vertices = new Dictionary<string, Wallet>();
            _adjacencyList = new Dictionary<string, List<Transaction>>();
        }

        // Düğüm (Cüzdan) ekler
        public void AddVertex(Wallet wallet)
        {
            if (!_vertices.ContainsKey(wallet.WalletId))
            {
                _vertices[wallet.WalletId] = wallet;
                _adjacencyList[wallet.WalletId] = new List<Transaction>();
            }
        }

        // Yönlü Kenar (İşlem/Transfer) ekler
        public void AddEdge(Transaction transaction)
        {
            // Gönderen cüzdan yoksa otomatik ekle
            if (!_adjacencyList.ContainsKey(transaction.FromWalletId))
            {
                AddVertex(new Wallet(transaction.FromWalletId));
            }

            // Alıcı cüzdan yoksa otomatik ekle
            if (!_adjacencyList.ContainsKey(transaction.ToWalletId))
            {
                AddVertex(new Wallet(transaction.ToWalletId));
            }

            // Yönlü kenar: sadece FromWallet -> ToWallet
            _adjacencyList[transaction.FromWalletId].Add(transaction);
        }

        // Düğüm (Cüzdan) siler ve ona bağlı tüm işlemleri temizler
        public void RemoveVertex(string walletId)
        {
            if (_vertices.ContainsKey(walletId))
            {
                _vertices.Remove(walletId);
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

        // BFS - Breadth First Search Algoritması
        // Belirli bir cüzdandan başlayan fon akışını katmanlı olarak takip eder
        public List<Transaction> BreadthFirstSearch(string startWalletId)
        {
            var result = new List<Transaction>();

            if (!_adjacencyList.ContainsKey(startWalletId))
                return result;

            var visited = new HashSet<string>(); // Ziyaret edilen cüzdanlar
            var queue = new Queue<string>();     // BFS kuyruğu

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

        // DFS - Depth First Search Algoritması
        // Belirli bir cüzdandan derinlemesine analiz yapar
        public List<Transaction> DepthFirstSearch(string startWalletId)
        {
            var result = new List<Transaction>();

            if (!_adjacencyList.ContainsKey(startWalletId))
                return result;

            var visited = new HashSet<string>(); // Ziyaret edilen cüzdanlar
            var stack = new Stack<string>();     // DFS yığıtı

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

        // Belirli bir cüzdanın tüm komşularını getirir
        public List<Transaction> GetNeighbors(string walletId)
        {
            if (_adjacencyList.ContainsKey(walletId))
                return _adjacencyList[walletId];

            return new List<Transaction>();
        }

        // Graf içindeki tüm cüzdanları getirir
        public IEnumerable<Wallet> GetAllWallets()
        {
            return _vertices.Values;
        }
    }
}