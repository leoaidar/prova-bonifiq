using ProvaPub.Models.Enums;
using ProvaPub.Services.Payments.Interfaces;

namespace ProvaPub.Services.Payments
{
    public class PixPaymentMethod : IPaymentMethod
    {
        public PaymentMethodEnum Method => PaymentMethodEnum.Pix;

        public async Task<bool> ProcessPaymentMethod(decimal paymentValue, int customerId)
        {
            //Faz pagamento...
            await Task.Delay(100); // tempo de processamento
            return true;
        }
    }
}
