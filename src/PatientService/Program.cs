using Data.DbContexts;
using MassTransit;
using MassTransit.DependencyInjection.Registration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService;
using PatientService.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Register MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<RegisterPatientFromIntegrationConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
            
        });
        cfg.ReceiveEndpoint("patient-registration-requests", e =>
        {
            e.ConcurrentMessageLimit = 5; // Load leveling behouden
            e.ConfigureConsumer<RegisterPatientFromIntegrationConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PatientManagementDb")));
var app = builder.Build();

// Endpoint accepting a raw string payload without a separate DTO

app.MapPatientEndPoints();
app.Run();
