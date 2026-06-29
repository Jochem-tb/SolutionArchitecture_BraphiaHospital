using Data.DbContexts;
using MassTransit;
using MedicalTreatmentApi;
using MedicalTreatmentApi.Consumers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<MedicalTreatmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MedicalTreatmentDb"))
);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AppointmentRescheduledConsumer>();
    x.AddConsumer<PatientCheckedInConsumer>();
    x.AddConsumer<SurveyCompletedConsumer>();
    x.AddConsumer<ProcedureDoneConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });

        cfg.ReceiveEndpoint("medical-treatment-service", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumers(context);
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapMedicalTreatmentEndpoints();
app.Run();
