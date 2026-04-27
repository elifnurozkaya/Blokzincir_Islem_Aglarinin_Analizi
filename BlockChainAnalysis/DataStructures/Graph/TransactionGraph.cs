using System;
using System.Collections.Generic;
using BlockChainAnalysis.Models;

namespace BlockChainAnalysis.DataStructures.Graph
{
    // 1. KİŞİ TARAFINDAN DOLDURULACAKTIR
    public class TransactionGraph
    {
        // TODO: Graf yapısını tutacak listeyi veya matrisi tanımlayın
        
        public TransactionGraph()
        {
            // TODO: Başlangıç atamalarını yapın
        }

        // Düğüm (Cüzdan) ekler
        public void AddVertex(Wallet wallet)
        {
            throw new NotImplementedException();
        }

        // Yönlü Kenar (İşlem/Transfer) ekler
        public void AddEdge(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        // BFS - Breadth First Search Algoritması
        // Belirli bir cüzdandan başlayan fon akışını katmanlı olarak takip eder
        public List<Transaction> BreadthFirstSearch(string startWalletId)
        {
            throw new NotImplementedException();
        }

        // DFS - Depth First Search Algoritması
        // Belirli bir cüzdandan derinlemesine analiz yapar
        public List<Transaction> DepthFirstSearch(string startWalletId)
        {
            throw new NotImplementedException();
        }
    }
}
