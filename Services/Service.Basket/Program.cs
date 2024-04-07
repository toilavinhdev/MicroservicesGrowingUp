using Discount.Grpc;
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
services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri("http://localhost:5002"); //Discount.Grpc
});

var app = builder.Build();
app.UseSwagger()
   .UseSwaggerUI(options =>
   {
       options.DocumentTitle = "Service.Basket";
   });
app.Map("/", () => "Service.Basket");

app.MapGet("/{userName}", 
    async (string userName, [FromServices]IDistributedCache redisCache) =>
    {
        var cart = await redisCache.GetStringAsync(userName);
        return cart is null 
            ? Results.BadRequest("Cart was not existed") 
            : Results.Ok(JsonConvert.DeserializeObject<Cart>(cart));
    });

app.MapPost("/create", 
    async (
        [FromBody]Cart cart, 
        [FromServices]IDistributedCache redisCache, [FromServices]DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient) =>
    {
        foreach (var cartItem in cart.Items)
        {
            var coupon = discountProtoServiceClient.GetDiscount(new GetDiscountRequest { ProductName = cartItem.ProductName });
            if (coupon is not null) cartItem.Price -= coupon.Amount;
        }
        await redisCache.SetStringAsync(cart.UserName, JsonConvert.SerializeObject(cart));
        return cart;
    });

app.MapDelete("/delete", 
    async (string userName, [FromServices]IDistributedCache redisCache) =>
    {
        await redisCache.RemoveAsync(userName);
    });

app.Run();