using CheckoutService.Model.Database.Enums;
using DynamicQL.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckoutService.Model.Database.Types
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        [ForeignKey(nameof(Payment))]
        public Guid? PaymentId { get; set; }
        public Payment? Payment { get; set; }
    }
}
