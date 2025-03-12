using ProvaPub.Models;
using ProvaPub.Models.Enums;
using ProvaPub.Repository;
using ProvaPub.Services.Payments;
using ProvaPub.Services.Payments.Interfaces;

namespace ProvaPub.Services
{
    public class OrderService
	{

        private readonly TestDbContext _ctx;
        private readonly Dictionary<PaymentMethodEnum, IPaymentMethod> _paymentMethods;

        public OrderService(TestDbContext ctx, IEnumerable<IPaymentMethod> paymentMethods)
        {
            _ctx = ctx;
            _paymentMethods = paymentMethods.ToDictionary(p => p.Method, p => p);
        }

        public async Task<Order> PayOrder(PaymentMethodEnum paymentMethod, decimal paymentValue, int customerId)
        {
            if (!_paymentMethods.TryGetValue(paymentMethod, out var paymentProcessor))
            {
                throw new ArgumentException("Invalid payment method.");
            }

            await paymentProcessor.ProcessPaymentMethod(paymentValue, customerId);

            return await InsertOrder(new Order
            {
                CustomerId = customerId,
                Value = paymentValue,
                OrderDate = DateTime.UtcNow // Data já salva em UTC no banco
            });
        }

        private async Task<Order> InsertOrder(Order order)
        {
            var newOrder = (await _ctx.Orders.AddAsync(order)).Entity;
            await _ctx.SaveChangesAsync();
            return newOrder;
        }
	}
}
