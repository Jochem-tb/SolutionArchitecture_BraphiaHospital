using MassTransit;
using PatientCare;
using PatientCare.Infrastructure.ReadDb;
using PatientCare.Infrastructure.WriteDb;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var writeDbConnectionString = builder.Configuration.GetConnectionString("PatientCareWriteDb");
var readDbConnectionString = builder.Configuration.GetConnectionString("PatientCareReadDb");

if (string.IsNullOrWhiteSpace(writeDbConnectionString) || string.IsNullOrWhiteSpace(readDbConnectionString))
{
    throw new InvalidOperationException("Missing PatientCare connection string configuration.");
}

builder.Services.AddDbContext<PatientCareWriteDbContext>(options =>
    options.UseSqlServer(writeDbConnectionString));

builder.Services.AddDbContext<PatientCareReadDbContext>(options =>
    options.UseSqlServer(readDbConnectionString));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin123");
        });

        cfg.ReceiveEndpoint("patient-care-service", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumers(context);
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var writeDb = scope.ServiceProvider.GetRequiredService<PatientCareWriteDbContext>();
    var readDb = scope.ServiceProvider.GetRequiredService<PatientCareReadDbContext>();

    writeDb.Database.Migrate();
    readDb.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapPatientCareEndpoints();
app.Run();


