using BuildingBlock.Messaging;
using MassTransit;

namespace Service.Ordering;

public class BasketOrderingConsumer(ILogger<BasketOrderingConsumer> logger, DataContext dataContext) : IConsumer<BasketCheckoutEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        await dataContext.Orders.AddAsync(new Order
        {
            FullName = "ORDER",
            UserName = context.Message.UserName,
            TotalPrice = context.Message.TotalPrice
        });
        await dataContext.SaveChangesAsync();
        logger.LogInformation($"Basket checkout event completed!!!");
    }
}