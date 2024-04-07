using System.Text.Json;
using MongoDB.Driver;
using Service.Catalog;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer().AddSwaggerGen();

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.Map("/", () => "Service.Catalog");

var client = new MongoClient("mongodb://localhost:27017");
var database = client.GetDatabase("MicroservicesGrowingUp");
var productCollection = database.GetCollection<Product>(nameof(Product));
var products = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText("products.json"));
foreach (var product in products ?? [])
{
    if (await productCollection.Find(x => x.Id == product.Id).AnyAsync()) continue;
    await productCollection.InsertOneAsync(product);
}


app.MapGet("/get-by-id", 
    async (string id) =>
    {
        return await productCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    });

app.MapGet("/list", 
    async (string? search, int pageIndex, int pageSize) =>
    {
        return await productCollection
            .Find(_ => true)
            .Skip(pageSize * (pageIndex - 1))
            .Limit(pageSize)
            .ToListAsync();
    });

app.MapPost("/create", 
    async (Product product) =>
    {
        await productCollection.InsertOneAsync(product);
    });

app.MapPut("/update",
    async (Product product) =>
    {
        var filter = Builders<Product>.Filter.Eq(x => x.Id, product.Id);
        var result = await productCollection.ReplaceOneAsync(filter, product);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    });

app.MapDelete("/delete", async (string id) =>
    {
        var result = await productCollection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    });

app.Run();
