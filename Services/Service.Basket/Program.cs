using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Service.Basket;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer().AddSwaggerGen();
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();
app.Map("/", () => "Service.Basket");

app.MapGet("/{userName}", 
    async (string userName, [FromServices]IDistributedCache redisCache) =>
    {
        var cart = await redisCache.GetStringAsync(userName);
        return cart is null 
            ? Results.BadRequest("Cart was not existed") 
            : Results.Ok(JsonConvert.DeserializeObject<Cart>(cart));
    });

app.MapGet("/list", 
    async ([FromServices]IDistributedCache redisCache) =>
    {
        var keys = await redisCache.GetStringAsync("*");
    });

app.MapPost("/create", 
    async (Cart cart, [FromServices]IDistributedCache redisCache) =>
    {
        await redisCache.SetStringAsync(cart.UserName, JsonConvert.SerializeObject(cart));
        return cart;
    });

app.MapDelete("/delete", 
    async (string userName, [FromServices]IDistributedCache redisCache) =>
    {
        await redisCache.RemoveAsync(userName);
    });

app.Run();