using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PatientService;

var builder = WebApplication.CreateBuilder(args);

// Register MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });
    });
});
var app = builder.Build();

// Endpoint accepting a raw string payload without a separate DTO

app.MapPatientEndPoints();
app.Run();
