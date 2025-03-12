using ProvaPub.Models;

namespace ProvaPub.Services
{
    public interface IProductService
    {
        public Task<ProductList> ListProductsAsync(int page);
    }
}
