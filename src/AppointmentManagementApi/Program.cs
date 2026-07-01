using AppointmentManagementApi;
using AppointmentManagementApi.Consumers;
using Data.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentManagementDb"))
);

builder.Services.AddEventStoreClient(builder.Configuration.GetConnectionString("EventStoreDb")!);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PatientRegisteredConsumer>();
    x.AddConsumer<PatientExaminedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });

        cfg.ReceiveEndpoint("Appointment-service", e =>
        {
            e.ConfigureConsumers(context);
        });

    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapAppointmentManagementEndpoints();

app.Run();