using DynamicQL.Attributes;

namespace CartService.Database.Model;

public class CartItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public byte Count { get; set; }
}