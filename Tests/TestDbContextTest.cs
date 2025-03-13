using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProvaPub.Tests
{
    public class TestDbContextTest : TestDbContext
    {
        private readonly List<Customer> _customers;
        private readonly List<Order> _orders;
        private readonly Dictionary<string, int> _countResults;

        public TestDbContextTest(DbContextOptions<TestDbContext> options) : base(options)
        {
            _customers = new List<Customer>();
            _orders = new List<Order>();
            _countResults = new Dictionary<string, int>();
        }

        public void SetupCustomer(Customer customer)
        {
            _customers.Add(customer);
        }

        public void SetupOrder(Order order)
        {
            _orders.Add(order);
        }

        public void SetupCountResult(string key, int count)
        {
            _countResults[key] = count;
        }

        public override ValueTask<TEntity?> FindAsync<TEntity>(params object?[]? keyValues) where TEntity : class
        {
            if (typeof(TEntity) == typeof(Customer))
            {
                var id = (int)keyValues[0];
                return new ValueTask<TEntity?>(_customers.FirstOrDefault(c => c.Id == id) as TEntity);
            }
            return new ValueTask<TEntity?>(default(TEntity));
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }

        public override int SaveChanges()
        {
            return 1;
        }

        public Task<int> CountAsync<TSource>(
            Expression<Func<TSource, bool>> predicate,
            CancellationToken cancellationToken = default) where TSource : class
        {
            var predicateString = predicate.ToString();
            
            if (typeof(TSource) == typeof(Order) && predicateString.Contains("OrderDate"))
            {
                return Task.FromResult(_countResults.GetValueOrDefault("OrdersInThisMonth", 0));
            }
            
            if (typeof(TSource) == typeof(Customer) && predicateString.Contains("Orders"))
            {
                return Task.FromResult(_countResults.GetValueOrDefault("CustomerHasBoughtBefore", 0));
            }
            
            return Task.FromResult(0);
        }
    }
}