using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private readonly DbContextOptions<TestDbContext> _options;

        public CustomerServiceTests()
        {
            // Configura o banco de dados em memória para os testes,
            // Nome único para evitar conflitos que tava tendo nos testes.
            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        // Método auxiliar para criar um novo contexto de teste
        private TestDbContext CreateContext() => new TestDbContext(_options);

        [Fact]
        public async Task Purchase_InvalidCustomerId_ThrowsException()
        {
            // ARRANGE: Cria o contexto DB, o provedor de data/hora e o serviço do cliente
            var context = CreateContext();
            var dateTimeProvider = new FakeDateTimeProvider(DateTime.UtcNow);
            var customerService = new CustomerService(context, dateTimeProvider);

            // ASSERT: Ids inválidos (0 ou negativo) tem que lançar Exception
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchase(0, 100));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchase(-1, 100));
        }

        [Fact]
        public async Task Purchase_InvalidValue_ThrowsException()
        {
            // ARRANGE: Inicializa o contexto, o provedor de data/hora e o serviço
            var context = CreateContext();
            var dateTimeProvider = new FakeDateTimeProvider(DateTime.UtcNow);
            var customerService = new CustomerService(context, dateTimeProvider);

            // ASSERT: Valores de compra inválidos (0 ou negativos) lança Exception
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchase(1, 0));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => customerService.CanPurchase(1, -10));
        }

        [Fact]
        public async Task Purchase_NonexistentCustomer_ThrowsException()
        {
            // ARRANGE: Cria o contexto e o serviço sem cadastrar o cliente de teste
            var context = CreateContext();
            var dateTimeProvider = new FakeDateTimeProvider(DateTime.UtcNow);
            var customerService = new CustomerService(context, dateTimeProvider);

            // ACT & ASSERT: Tentar fazer a compra com um cliente inexistente deve lançar operacao invalida
            await Assert.ThrowsAsync<InvalidOperationException>(() => customerService.CanPurchase(999, 100));
        }

        [Fact]
        public async Task Purchase_RecentOrder_Fails()
        {
            // ARRANGE: Define o Id e a data atual para o teste
            int customerId = 1;
            var currentDate = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Utc);
            var dateTimeProvider = new FakeDateTimeProvider(currentDate);

            // Insere um cliente e um pedido realizado há 15 dias (dentro do último mês)
            using (var context = CreateContext())
            {
                var customer = new Customer { Id = customerId, Name = "Test Customer" };
                context.Customers.Add(customer);

                var orderDate = currentDate.AddDays(-15); // Pedido realizado 15 dias atrás
                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = orderDate,
                    Value = 50
                };
                context.Orders.Add(order);
                await context.SaveChangesAsync();
            }

            // ACT: Verifica se o cliente com pedido recente não pode realizar nova compra
            using (var context = CreateContext())
            {
                var customerService = new CustomerService(context, dateTimeProvider);
                var result = await customerService.CanPurchase(customerId, 50);

                // ASSERT: O resultado deve ser false, pois o cliente já possui pedido recente
                Assert.False(result);
            }
        }

        [Fact]
        public async Task Purchase_FirstTimeOverLimit_Fails()
        {
            // ARRANGE: id cliente e a data corrente
            int customerId = 2;
            var currentDate = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Utc);
            var dateTimeProvider = new FakeDateTimeProvider(currentDate);

            // Novo cliente sem pedidos anteriores
            using (var context = CreateContext())
            {
                var customer = new Customer { Id = customerId, Name = "New Customer" };
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }

            // ACT: Tenta fazer uma compra com valor maior que 100 para um cliente que faz a primeira compra
            using (var context = CreateContext())
            {
                var customerService = new CustomerService(context, dateTimeProvider);
                var result = await customerService.CanPurchase(customerId, 150); // 150 > 100

                // ASSERT: Compra negada 
                Assert.False(result);
            }
        }

        [Fact]
        public async Task Purchase_FirstTimeWithinLimit_Succeeds()
        {
            // ARRANGE: Define o ID do novo cliente e uma data em horário comercial (10 AM)
            int customerId = 3;
            var currentDate = new DateTime(2023, 5, 15, 10, 0, 0, DateTimeKind.Utc);
            var dateTimeProvider = new FakeDateTimeProvider(currentDate);

            // Insere um novo cliente sem pedidos anteriores
            using (var context = CreateContext())
            {
                var customer = new Customer { Id = customerId, Name = "New Customer" };
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }

            // ACT: Tenta fazer uma compra com valor igual a 100
            using (var context = CreateContext())
            {
                var customerService = new CustomerService(context, dateTimeProvider);
                var result = await customerService.CanPurchase(customerId, 100);

                // ASSERT: A compra foi permitida 
                Assert.True(result);
            }
        }

        [Fact]
        public async Task Purchase_Existing_NoRecentOrder_Succeeds()
        {
            // ARRANGE: Define o Id do cliente e a data atual em horário comercial (2 PM)
            int customerId = 4;
            var currentDate = new DateTime(2023, 5, 15, 14, 0, 0, DateTimeKind.Utc);
            var dateTimeProvider = new FakeDateTimeProvider(currentDate);

            // Insere um cliente com um pedido feito há 2 meses (fora do período de bloqueio)
            using (var context = CreateContext())
            {
                var customer = new Customer { Id = customerId, Name = "Existing Customer" };
                context.Customers.Add(customer);

                var orderDate = currentDate.AddMonths(-2);
                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = orderDate,
                    Value = 200
                };
                context.Orders.Add(order);
                await context.SaveChangesAsync();
            }

            // ACT: Verifica se o cliente pode fazer  nova compra com valor alto (500)
            using (var context = CreateContext())
            {
                var customerService = new CustomerService(context, dateTimeProvider);
                var result = await customerService.CanPurchase(customerId, 500); // 500 > 100

                // ASSERT: Compra permitida 
                Assert.True(result);
            }
        }

        [Fact]
        public async Task Purchase_OutsideBusinessHours_Fails()
        {
            // ARRANGE: Define o Id do cliente e uma data fora do horário comercial (8 PM)
            int customerId = 5;
            var currentDate = new DateTime(2023, 5, 15, 20, 0, 0, DateTimeKind.Utc);
            var dateTimeProvider = new FakeDateTimeProvider(currentDate);

            // Insere o cliente
            using (var context = CreateContext())
            {
                var customer = new Customer { Id = customerId, Name = "Customer" };
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }

            // ACT: Tenta fazer uma compra fora do horário comercial
            using (var context = CreateContext())
            {
                var customerService = new CustomerService(context, dateTimeProvider);
                var result = await customerService.CanPurchase(customerId, 50);

                // ASSERT: A compra não foi autorizada
                Assert.False(result);
            }
        }

        [Fact]
        public async Task Purchase_Weekend_Fails()
        {
            // ARRANGE: Define o ID cliente e data que cai sábado
            int customerId = 6;
            var currentDate = new DateTime(2023, 5, 13, 12, 0, 0, DateTimeKind.Utc);
            var dateTimeProvider = new FakeDateTimeProvider(currentDate);

            // Insere o cliente
            using (var context = CreateContext())
            {
                var customer = new Customer { Id = customerId, Name = "Weekend Customer" };
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }

            // ACT: Tenta realizar uma compra no final de semana
            using (var context = CreateContext())
            {
                var customerService = new CustomerService(context, dateTimeProvider);
                var result = await customerService.CanPurchase(customerId, 50);

                // ASSERT: Compra não permitida
                Assert.False(result);
            }
        }
    }
}
