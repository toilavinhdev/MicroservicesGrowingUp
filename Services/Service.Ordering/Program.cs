using BuildingBlock.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Service.Ordering;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer().AddSwaggerGen();
services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer("Server=localhost; Database=MicroservicesGrowingUp; Integrated Security= True;Trust Server Certificate= True");
});
services.AddMassTransit(registrationConfig =>
{
    registrationConfig.AddConsumer<BasketOrderingConsumer>();
    registrationConfig.UsingRabbitMq((ctx, factoryConfig) =>
    {
        factoryConfig.Host("amqp://guest:guest@localhost:5672");
        factoryConfig.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, cfg =>
        {
            cfg.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });
    });
});

var app = builder.Build();
app.UseSwagger()
   .UseSwaggerUI(options =>
   {
       options.DocumentTitle = "Service.Ordering";
   });
app.Map("/", () => "Service.Ordering");

app.MapGet("/list",
    async (DataContext context) => await context.Orders.ToListAsync());

app.MapGet("/{userName}", async (string userName, DataContext context) =>
    {
        var order = await context.Orders.FirstOrDefaultAsync(x => x.UserName.Equals(userName));
        return order is null 
            ? Results.BadRequest("Order was not existed") 
            : Results.Ok(order);
    });

app.Run();