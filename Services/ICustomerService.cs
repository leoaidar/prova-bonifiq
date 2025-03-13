using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IActionResult> GetAllAsync(int page);
        Task<bool> CanPurchase(int customerId, decimal purchaseValue);
    }
}
