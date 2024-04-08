namespace BuildingBlock.Messaging;

public class BasketCheckoutEvent : BaseIntegrationEvent
{
    public string UserName { get; set; } = default!;
    
    public decimal TotalPrice { get; set; }
}