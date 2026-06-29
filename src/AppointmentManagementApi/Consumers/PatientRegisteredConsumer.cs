using Data.Contracts;
using Data.DbContexts;
using Data.Models;
using MassTransit;

namespace AppointmentManagementApi.Consumers;

public class PatientRegisteredConsumer : IConsumer<PatientRegistered>
{
    private readonly ILogger<PatientRegisteredConsumer> _logger;
    private readonly AppointmentDbContext _appointmentDbContext;

    public PatientRegisteredConsumer(ILogger<PatientRegisteredConsumer> logger, AppointmentDbContext appointmentDbContext)
    {
        _logger = logger;
        _appointmentDbContext = appointmentDbContext;
    }

    public async Task Consume(ConsumeContext<PatientRegistered> context)
    {
        _logger.LogInformation("Patient Registered: {Id} - Name: {Name} at {Time}",
            context.Message.PatientId, context.Message.Name, context.Message.RegisteredAt);

        _appointmentDbContext.Patients.Add(
                new PatientSmall()
                {
                    Id = context.Message.PatientId,
                    Name = context.Message.Name,
                }
            );

        await _appointmentDbContext.SaveChangesAsync();
    }
}