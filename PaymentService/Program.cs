using EventBus;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Consumer;
using PaymentService.DbContexts;
using PaymentService.Interfaces;
using PaymentService.Services.VnPay;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<PaymentDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var assembly = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(assembly);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IVnPayService, VnPayService>();

builder.Services.AddScoped<IEventBus, MessageBus.EventBus>();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    busConfigurator.AddConsumer<OrderCreatedConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["MessageBroker:Host"], "/", h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        configurator.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapControllers();
}

app.UseHttpsRedirection();

app.Run();

