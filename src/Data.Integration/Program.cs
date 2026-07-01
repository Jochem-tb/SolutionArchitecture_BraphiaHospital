using Data.Integration; 
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddScoped<PatientFileImporter>();
var app = builder.Build();
app.MapIntegrationEndpoints();
app.Run();