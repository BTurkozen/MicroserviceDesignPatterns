using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order.Api.Consumers;
using Order.Api.Models;
using Shared.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionStr"));
});

builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<PaymentCompletedEventConsumer>();
    options.AddConsumer<PaymentFailedEventConsumer>();
    options.AddConsumer<StockNotreservedEventConsumer>();

    options.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("RabbitMq"));

        config.ReceiveEndpoint(RabbitMqSettingsConst.OrderPaymentCompletedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentCompletedEventConsumer>(context);
        });

        config.ReceiveEndpoint(RabbitMqSettingsConst.OrderPaymentFailedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });

        config.ReceiveEndpoint(RabbitMqSettingsConst.StockNotReservedEventQueueName, e =>
        {
            e.ConfigureConsumer<StockNotreservedEventConsumer>(context);
        });
    });
});

builder.Services.AddSingleton(typeof(ILogger), builder.Services.BuildServiceProvider().GetService<ILogger<PaymentCompletedEventConsumer>>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();