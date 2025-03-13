using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;
using ProvaPub.Utils;
using System;
using System.Threading.Tasks;

namespace ProvaPub.Services
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        protected readonly IDateTimeProvider _dateTimeProvider;

        public CustomerService(TestDbContext ctx, IDateTimeProvider? dateTimeProvider = null) : base(ctx)
        {
            _dateTimeProvider = dateTimeProvider ?? new SystemDateTimeProvider();
        }

        public virtual async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var currentDateTime = _dateTimeProvider.UtcNow;
            var baseDate = currentDateTime.AddMonths(-1);
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            var hour = currentDateTime.Hour;
            var dayOfWeek = currentDateTime.DayOfWeek;
            if (hour < 8 || hour > 18 || 
                dayOfWeek == DayOfWeek.Saturday || 
                dayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }
    }
}
