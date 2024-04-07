namespace Service.Basket;

public class Cart
{
    public string UserName { get; set; } = default!;

    public List<CartItem> Items { get; set; } = default!;
}

public class CartItem
{
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }

    public string ProductId { get; set; } = default!;
    
    public string ImageFile { get; set; } = default!;
}