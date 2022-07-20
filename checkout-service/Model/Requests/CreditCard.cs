using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CheckoutService.Model.Requests
{
    public class CreditCard
    {
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string CardExpirationDate { get; set; }
        [Required]
        public string Pin { get; set; }

        public bool IsCreditCardInfoValid()
        {
            var cardCheck = new Regex(@"^(1298|1267|4512|4567|8901|8933)([\-\s]?[0-9]{4}){3}$");
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");
            var cvvCheck = new Regex(@"^\d{3}$");

            if (!cardCheck.IsMatch(this.CardNumber)) // <1>check card number is valid
                return false;
            if (!cvvCheck.IsMatch(this.Pin)) // <2>check cvv is valid as "999"
                return false;

            var dateParts = this.CardExpirationDate.Split('/'); //expiry date in from MM/yyyy            
            if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1])) // <3 - 6>
                return false; // ^ check date format is valid as "MM/yyyy"

            var year = int.Parse(dateParts[1]);
            var month = int.Parse(dateParts[0]);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            //check expiry greater than today & within next 6 years <7, 8>>
            return (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6));
        }
    }
}
