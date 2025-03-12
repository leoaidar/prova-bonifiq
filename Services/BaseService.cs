using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class BaseService<T> where T : class
    {
        protected readonly TestDbContext _ctx;
        protected readonly DbSet<T> _dbSet;
        protected readonly int _pageSize = 10;

        public BaseService(TestDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<T>();
        }

        public async Task<IActionResult> GetAllAsync(int page)
        {
            var items = await Task.FromResult(_dbSet.AsQueryable());

            var paginated = Pagination<T>.Create(items, page, _pageSize);

            return await Task.FromResult(new OkObjectResult(paginated));
        }
    }
}
