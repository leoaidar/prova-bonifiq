using ProvaPub.Models.Enums;

namespace ProvaPub.Services.Payments.Interfaces
{
    public interface IPaymentMethod
    {
        PaymentMethodEnum Method { get; }
        Task<bool> ProcessPaymentMethod(decimal paymentValue, int customerId);
    }
}
