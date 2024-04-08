using BuildingBlock.Messaging;
using Discount.Grpc;
using MassTransit;
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
services.AddMassTransit(registrationConfig =>
{
    registrationConfig.UsingRabbitMq((ctx, factoryConfig) =>
    {
        factoryConfig.Host("amqp://guest:guest@localhost:5672");
    });
});

var app = builder.Build();
app.UseSwagger()
   .UseSwaggerUI(options =>
   {
       options.DocumentTitle = "Service.Basket";
   });
app.Map("/", () => "Service.Basket");

var group = app.MapGroup("/basket/api");

group.MapGet("/{userName}", 
    async (string userName, [FromServices]IDistributedCache redisCache) =>
    {
        var cart = await redisCache.GetStringAsync(userName);
        return cart is null 
            ? Results.BadRequest("Cart was not existed") 
            : Results.Ok(JsonConvert.DeserializeObject<Cart>(cart));
    });

group.MapPost("/create", 
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

group.MapDelete("/delete", 
    async (string userName, [FromServices]IDistributedCache redisCache) =>
    {
        await redisCache.RemoveAsync(userName);
    });

group.MapPost("/checkout",
    async (
        string userName,
        [FromServices]IDistributedCache redisCache, [FromServices]IPublishEndpoint publishEndpoint) =>
    {
        var cartValue = await redisCache.GetStringAsync(userName);
        if (cartValue is null) return Results.BadRequest("Cart was not existed");
        var cart = JsonConvert.DeserializeObject<Cart>(cartValue);
        if (cart is null) throw new NullReferenceException("Cart was not nullable");

        await publishEndpoint.Publish(
            new BasketCheckoutEvent
            {
                UserName = cart.UserName,
                TotalPrice = cart.Items.Aggregate((decimal)0, (acc, cur) => acc + (cur.Price * cur.Quantity))
            });

        await redisCache.RemoveAsync(userName);

        return Results.Ok("Checkout successfully");
    });

app.Run();