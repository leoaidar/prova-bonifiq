using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces
{
    public interface ICustomerService
    {
        public Task<IActionResult> GetAllAsync(int page);
    }
}
