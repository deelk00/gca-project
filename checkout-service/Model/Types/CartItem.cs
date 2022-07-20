namespace CheckoutService.Model.Types
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public byte Count { get; set; }
    }
}
