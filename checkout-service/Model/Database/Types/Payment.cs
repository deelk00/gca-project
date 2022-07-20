namespace CheckoutService.Model.Database.Types
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string CreditCardNumber { get; set; }
        public string CreditCardExpirationDate { get; set; }
        public string Pin { get; set; }

        private DateTimeOffset? _datePayment;
        public DateTimeOffset PaymentDate 
        {
            get => _datePayment ?? DateTimeOffset.Now; 
            set
            {
                if (_datePayment != null) throw new Exception("Paymentdate can't be changed");
                _datePayment = value;
            } 
        }
    }
}
