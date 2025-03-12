using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using System;

namespace ProvaPub.Services
{
	public class RandomService
    {

        private readonly TestDbContext _ctx;

        public RandomService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<int> GetRandom()
		{
            var random = new Random(DateTime.Now.Millisecond);
            var number =  new Random().Next(100);
            
            while (await _ctx.Numbers.AnyAsync(n => n.Number == number))
            {
                number = random.Next(100);
            }

            _ctx.Numbers.Add(new RandomNumber() { Number = number });
            await _ctx.SaveChangesAsync();

            return number;
		}

	}
}
