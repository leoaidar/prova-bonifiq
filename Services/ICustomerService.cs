using ProvaPub.Models;

namespace ProvaPub.Services
{
    public interface ICustomerService
    {
        public Task<CustomerList> ListCustomersAsync(int page);
    }
}
