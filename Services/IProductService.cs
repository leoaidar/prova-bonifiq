using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces
{
    public interface IProductService
    {
        public Task<IActionResult> GetAllAsync(int page);
    }
}
