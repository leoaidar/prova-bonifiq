using Microsoft.EntityFrameworkCore;
using ProvaPub.Configs;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using ProvaPub.Services.Payments;
using ProvaPub.Services.Payments.Interfaces;
using ProvaPub.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumSchemaFilter>();
});

// Registro dos serviços com injeção de dependência
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<RandomService>();
builder.Services.AddScoped<OrderService>();

// Registra os métodos de pagamento
builder.Services.AddScoped<IPaymentMethod, PixPaymentMethod>();
builder.Services.AddScoped<IPaymentMethod, CreditCardPaymentMethod>();
builder.Services.AddScoped<IPaymentMethod, PayPalPaymentMethod>();

// Registra o provedor de data/hora
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

// Registra o banco de dados
builder.Services.AddDbContext<TestDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ctx")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
