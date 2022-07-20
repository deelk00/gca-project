namespace CheckoutService.Model.Types
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<CartItem> CartItems { get; set; }
        public Guid? OrderId { get; set; }
        private DateTimeOffset? _atCreated;

        public DateTimeOffset CreatedAt { get; set; }
    }
}
