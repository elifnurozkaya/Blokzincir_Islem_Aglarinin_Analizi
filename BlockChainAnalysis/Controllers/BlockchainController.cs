using Microsoft.AspNetCore.Mvc;
using BlockChainAnalysis.Models;
using BlockChainAnalysis.DataStructures.Graph;
using BlockChainAnalysis.DataStructures.HashTable;
using BlockChainAnalysis.DataStructures.Tree;
using System.Collections.Generic;

namespace BlockChainAnalysis.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockchainController : ControllerBase
    {
        private readonly TransactionGraph  _graph;
        private readonly WalletHashTable   _hashTable;
        private readonly MerkleTree        _merkleTree;

        // Dependency Injection: Program.cs'te singleton olarak kayıtlı servisler inject edilir
        public BlockchainController(
            TransactionGraph graph,
            WalletHashTable  hashTable,
            MerkleTree       merkleTree)
        {
            _graph      = graph;
            _hashTable  = hashTable;
            _merkleTree = merkleTree;
        }

        // ================================================================
        // CÜZDAN (WALLET) ENDPOINTLERİ
        // ================================================================

        /// <summary>
        /// Yeni bir cüzdan ekler (Hash Table'a kaydeder).
        /// POST /api/blockchain/wallet
        /// Body: { "walletId": "abc123", "balance": 500.0 }
        /// </summary>
        [HttpPost("wallet")]
        public IActionResult AddWallet([FromBody] Wallet wallet)
        {
            if (wallet == null || string.IsNullOrWhiteSpace(wallet.WalletId))
                return BadRequest("Geçerli bir cüzdan bilgisi gönderilmedi.");

            // Önceden bu cüzdan varsa hata dön (Overwrite engeli)
            if (_hashTable.Search(wallet.WalletId) != null)
                return BadRequest($"Hata: '{wallet.WalletId}' ID'li cüzdan zaten sistemde mevcut!");

            _hashTable.Insert(wallet.WalletId, wallet);
            _graph.AddVertex(wallet);

            return Ok(new { message = $"Cüzdan eklendi: {wallet.WalletId}" });
        }

        /// <summary>
        /// Hash Table üzerinden O(1) ile cüzdan arar.
        /// GET /api/blockchain/wallet/{walletId}
        /// </summary>
        [HttpGet("wallet/{walletId}")]
        public IActionResult GetWallet(string walletId)
        {
            var wallet = _hashTable.Search(walletId);

            if (wallet == null)
                return NotFound(new { message = $"Cüzdan bulunamadı: {walletId}" });

            return Ok(wallet);
        }

        /// <summary>
        /// Cüzdanı Hash Table'dan ve Graftan siler.
        /// DELETE /api/blockchain/wallet/{walletId}
        /// </summary>
        [HttpDelete("wallet/{walletId}")]
        public IActionResult DeleteWallet(string walletId)
        {
            var wallet = _hashTable.Search(walletId);
            if (wallet == null)
                return NotFound(new { message = $"Hata: Silinmek istenen '{walletId}' ID'li cüzdan bulunamadı!" });

            _hashTable.Delete(walletId);
            _graph.RemoveVertex(walletId); // Graf'tan da cüzdanı ve ona bağlı tüm işlemleri uçuruyoruz
            RebuildMerkleTree();           // İşlemler silindiği için Merkle Tree'yi yeniden inşa et

            return Ok(new { message = $"Cüzdan ve ona bağlı işlemler silindi: {walletId}" });
        }

        // ================================================================
        // İŞLEM (TRANSACTION) ENDPOINTLERİ
        // ================================================================

        /// <summary>
        /// Yeni bir işlem (transfer) ekler; Graf'a kenar, Merkle Tree'ye yaprak olarak girer.
        /// POST /api/blockchain/transaction
        /// Body: { "transactionId": "tx1", "fromWalletId": "A", "toWalletId": "B", "amount": 100, "timestamp": "2024-01-01T00:00:00" }
        /// </summary>
        [HttpPost("transaction")]
        public IActionResult AddTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
                return BadRequest("Geçerli bir işlem gönderilmedi.");

            if (string.IsNullOrWhiteSpace(transaction.FromWalletId) ||
                string.IsNullOrWhiteSpace(transaction.ToWalletId))
                return BadRequest("Gönderen ve alıcı cüzdan ID'leri boş olamaz.");

            // Graf'a yönlü kenar olarak ekle
            _graph.AddEdge(transaction);

            // Merkle Tree'yi yeniden inşa et (tüm işlemleri içerecek şekilde)
            RebuildMerkleTree();

            return Ok(new
            {
                message       = "İşlem eklendi.",
                transactionId = transaction.TransactionId,
                merkleRoot    = _merkleTree.GetRootHash()
            });
        }

        // ================================================================
        // GRAF DOLAŞIM ENDPOINTLERİ
        // ================================================================

        /// <summary>
        /// BFS: Belirli bir cüzdandan başlayarak fon akışını katman katman takip eder.
        /// GET /api/blockchain/bfs/{walletId}
        /// </summary>
        [HttpGet("bfs/{walletId}")]
        public IActionResult BreadthFirstSearch(string walletId)
        {
            var transactions = _graph.BreadthFirstSearch(walletId);

            if (transactions.Count == 0)
                return NotFound(new { message = $"Bu cüzdandan çıkan işlem bulunamadı: {walletId}" });

            return Ok(new
            {
                startWallet  = walletId,
                algorithm    = "BFS",
                count        = transactions.Count,
                transactions
            });
        }

        /// <summary>
        /// DFS: Belirli bir cüzdandan derinlemesine fon akışı analizi yapar.
        /// GET /api/blockchain/dfs/{walletId}
        /// </summary>
        [HttpGet("dfs/{walletId}")]
        public IActionResult DepthFirstSearch(string walletId)
        {
            var transactions = _graph.DepthFirstSearch(walletId);

            if (transactions.Count == 0)
                return NotFound(new { message = $"Bu cüzdandan çıkan işlem bulunamadı: {walletId}" });

            return Ok(new
            {
                startWallet  = walletId,
                algorithm    = "DFS",
                count        = transactions.Count,
                transactions
            });
        }

        /// <summary>
        /// Belirli bir cüzdanın doğrudan komşularını (transfer yaptığı cüzdanları) listeler.
        /// GET /api/blockchain/neighbors/{walletId}
        /// </summary>
        [HttpGet("neighbors/{walletId}")]
        public IActionResult GetNeighbors(string walletId)
        {
            var neighbors = _graph.GetNeighbors(walletId);
            return Ok(new { walletId, neighbors });
        }

        /// <summary>
        /// Graftaki tüm cüzdanları listeler.
        /// GET /api/blockchain/wallets
        /// </summary>
        [HttpGet("wallets")]
        public IActionResult GetAllWallets()
        {
            var wallets = _graph.GetAllWallets();
            return Ok(wallets);
        }

        // ================================================================
        // MERKLE TREE ENDPOINTLERİ
        // ================================================================

        /// <summary>
        /// Merkle Root hash değerini döndürür.
        /// GET /api/blockchain/merkle/root
        /// </summary>
        [HttpGet("merkle/root")]
        public IActionResult GetMerkleRoot()
        {
            var root = _merkleTree.GetRootHash();

            if (root == null)
                return NotFound(new { message = "Henüz hiç işlem eklenmedi; Merkle Tree boş." });

            return Ok(new { merkleRoot = root });
        }

        /// <summary>
        /// Verilen işlem listesinin mevcut Merkle Root ile eşleşip eşleşmediğini doğrular.
        /// POST /api/blockchain/merkle/verify
        /// Body: [ { transaction1 }, { transaction2 }, ... ]
        /// </summary>
        [HttpPost("merkle/verify")]
        public IActionResult VerifyIntegrity()
        {
            // Tüm işlemleri toplayıp genel Merkle Root ile kıyaslamamız lazım. (Arayüzden gelen eksik liste ile değil)
            var allTransactions = new List<Transaction>();
            foreach (var wallet in _graph.GetAllWallets())
            {
                allTransactions.AddRange(_graph.GetNeighbors(wallet.WalletId));
            }

            bool isValid = _merkleTree.VerifyIntegrity(allTransactions);

            return Ok(new
            {
                isValid,
                message = isValid
                    ? "Veri bütünlüğü doğrulandı. İşlemler değiştirilmemiş."
                    : "UYARI: Veri bütünlüğü bozulmuş! İşlemler değiştirilmiş olabilir."
            });
        }

        // ================================================================
        // YARDIMCI: Merkle Tree'yi yeniden inşa et
        // ================================================================
        // Not: Gerçek bir sistemde tüm transaction'lar bir repository'de tutulur.
        // Şimdilik in-memory olarak Graf'tan toplanıyor.
        private void RebuildMerkleTree()
        {
            // Graf'taki tüm işlemleri topla
            var allTransactions = new List<Transaction>();
            foreach (var wallet in _graph.GetAllWallets())
            {
                foreach (var tx in _graph.GetNeighbors(wallet.WalletId))
                {
                    allTransactions.Add(tx);
                }
            }

            _merkleTree.BuildTree(allTransactions);
        }
    }
}