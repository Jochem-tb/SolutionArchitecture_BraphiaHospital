using MassTransit;
using PatientCare;
using PatientCare.Commands.Abstractions;
using PatientCare.Commands.AddMedicalHistoryEntry;
using PatientCare.Commands.CreatePrescription;
using PatientCare.Commands.UpsertAppointmentCache;
using PatientCare.Commands.UpsertPatientCache;
using PatientCare.Consumers;
using PatientCare.Consumers.ReadProjections;
using PatientCare.Infrastructure.ReadDb;
using PatientCare.Infrastructure.ReadDb.Projections;
using PatientCare.Infrastructure.WriteDb.Repositories;
using PatientCare.Infrastructure.WriteDb;
using PatientCare.Queries.GetMedicalDossier;
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

builder.Services.AddScoped<IPatientCareWriteRepository, PatientCareWriteRepository>();
builder.Services.AddScoped<ICommandHandler<AddMedicalHistoryEntryCommand, Guid>, AddMedicalHistoryEntryCommandHandler>();
builder.Services.AddScoped<ICommandHandler<CreatePrescriptionCommand, Guid>, CreatePrescriptionCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpsertPatientCacheCommand, Guid>, UpsertPatientCacheCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpsertAppointmentCacheCommand, Guid>, UpsertAppointmentCacheCommandHandler>();
builder.Services.AddScoped<IPatientCareReadProjector, PatientCareReadProjector>();
builder.Services.AddScoped<GetMedicalDossierQueryHandler>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PatientRegisteredConsumer>();
    x.AddConsumer<AppointmentPlannedConsumer>();
    x.AddConsumer<PatientExaminedConsumer>();
    x.AddConsumer<MedicationPrescribedConsumer>();
    x.AddConsumer<ProjectionConsumer>();

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
            e.ConfigureConsumer<PatientRegisteredConsumer>(context);
            e.ConfigureConsumer<AppointmentPlannedConsumer>(context);
            e.ConfigureConsumer<PatientExaminedConsumer>(context);
            e.ConfigureConsumer<MedicationPrescribedConsumer>(context);
        });

        cfg.ReceiveEndpoint("patient-care-read-projections", e =>
        {
            e.Durable = true;
            e.AutoDelete = false;

            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

            e.ConfigureConsumer<ProjectionConsumer>(context);
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var writeDb = scope.ServiceProvider.GetRequiredService<PatientCareWriteDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    writeDb.Database.Migrate();

    try
    {
        var readDb = scope.ServiceProvider.GetRequiredService<PatientCareReadDbContext>();
        readDb.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "ReadDb migration skipped because ReadDb is currently unavailable. Projection consumers will retry when ReadDb is online.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapPatientCareEndpoints();
app.Run();


