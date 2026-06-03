using BlockChainAnalysis.DataStructures.Graph;
using BlockChainAnalysis.DataStructures.HashTable;
using BlockChainAnalysis.DataStructures.Tree;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<TransactionGraph>();
builder.Services.AddSingleton<WalletHashTable>();
builder.Services.AddSingleton<MerkleTree>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ================================================================
// DUMMY DATA SEEDING (1'den 10'a kadar cüzdan ve örnek işlemler)
// ================================================================
using (var scope = app.Services.CreateScope())
{
    var graph = scope.ServiceProvider.GetRequiredService<TransactionGraph>();
    var hashTable = scope.ServiceProvider.GetRequiredService<WalletHashTable>();
    var merkleTree = scope.ServiceProvider.GetRequiredService<MerkleTree>();
    var transactions = new System.Collections.Generic.List<BlockChainAnalysis.Models.Transaction>();

    // 10 Cüzdan Ekle
    for (int i = 1; i <= 10; i++)
    {
        var w = new BlockChainAnalysis.Models.Wallet($"Cüzdan-{i}") { Balance = 1000 };
        hashTable.Insert(w.WalletId, w);
        graph.AddVertex(w);
    }

    // Örnek 15 Transfer (Karmaşık bir ağ görüntüsü için)
    var rnd = new Random(42);
    for (int i = 1; i <= 15; i++)
    {
        int from = rnd.Next(1, 11);
        int to = rnd.Next(1, 11);
        while (from == to) to = rnd.Next(1, 11);

        var tx = new BlockChainAnalysis.Models.Transaction(
            $"TX-00{i}",
            $"Cüzdan-{from}",
            $"Cüzdan-{to}",
            rnd.Next(10, 100),
            DateTime.UtcNow.AddMinutes(-i)
        );
        
        graph.AddEdge(tx);
        transactions.Add(tx);
    }

    // Merkle Tree'yi Graf üzerinden toplayarak inşa et (Doğrulama sırasının Controller ile birebir aynı olması zorunludur)
    var allTxFromGraph = new System.Collections.Generic.List<BlockChainAnalysis.Models.Transaction>();
    foreach (var wallet in graph.GetAllWallets())
    {
        allTxFromGraph.AddRange(graph.GetNeighbors(wallet.WalletId));
    }
    merkleTree.BuildTree(allTxFromGraph);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
