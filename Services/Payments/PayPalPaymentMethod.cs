using ProvaPub.Models.Enums;
using ProvaPub.Services.Payments.Interfaces;

namespace ProvaPub.Services.Payments
{
    public class PayPalPaymentMethod : IPaymentMethod
    {
        public PaymentMethodEnum Method => PaymentMethodEnum.PayPal;

        public async Task<bool> ProcessPaymentMethod(decimal paymentValue, int customerId)
        {
            //Faz pagamento...
            await Task.Delay(1000); // tempo de processamento
            return true;
        }
    }
}
