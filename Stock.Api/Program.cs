using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.OrderEvents;
using Shared.Settings;
using Stock.Api.Consumers;
using Stock.Api.Models;
using System.Linq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseInMemoryDatabase("SagaChoreagraphyStockDb");
});

builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<OrderCreatedEventConsumer>();

    options.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("RabbitMq"));

        config.ReceiveEndpoint(RabbitMqSettingsConst.StockOrderCreatedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
    });
});

//builder.Services.AddLogging();

var logger = builder.Services.BuildServiceProvider().GetService<ILogger<OrderCreatedEventConsumer>>();

builder.Services.AddSingleton(typeof(ILogger), logger);

var app = builder.Build();

using var scope = app.Services.CreateScope();

var serviceProvider = scope.ServiceProvider;

var dataContext = serviceProvider.GetRequiredService<DataContext>();

if (dataContext.Stocks.Any() is false)
{
    dataContext.Stocks.Add(new Stock.Api.Models.Stock() { Id = 1, ProductId = 1, Count = 100 });

    dataContext.Stocks.Add(new Stock.Api.Models.Stock() { Id = 2, ProductId = 2, Count = 200 });

    dataContext.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();