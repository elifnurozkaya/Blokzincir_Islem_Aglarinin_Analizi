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
