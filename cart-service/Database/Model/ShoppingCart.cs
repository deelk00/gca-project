using System.ComponentModel.DataAnnotations.Schema;
using DynamicQL.Attributes;

namespace CartService.Database.Model;

[DynamicQL]
public class ShoppingCart
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<CartItem> CartItems { get; set; }

    private DateTimeOffset? _atCreated;
    
    public DateTimeOffset CreatedAt
    {
        get => _atCreated ?? DateTimeOffset.Now;
        set
        {
            if (_atCreated != null) throw new Exception("can't mutate creation date");
            _atCreated = value;
        }
    }
}