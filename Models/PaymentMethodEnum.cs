using System.ComponentModel.DataAnnotations;

namespace ProvaPub.Models.Enums
{
    public enum PaymentMethodEnum
    {
        [Display(Name = "Pix")]
        Pix = 1,

        [Display(Name = "CreditCard")]
        CreditCard = 2,

        [Display(Name = "PayPal")]
        PayPal = 3
    }
}
