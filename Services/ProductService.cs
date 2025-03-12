using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class ProductService : IProductService
	{
        private readonly TestDbContext _ctx;

        public ProductService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ProductList> ListProductsAsync(int page)
		{
			var items = await Task.FromResult(_ctx.Products.AsQueryable());

			var paginated = Pagination<Product>.Create(items, page);

            return new ProductList { Result = paginated };
		}
	}
}
