using Data.Contracts;
using Data.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MedicalTreatmentApi.Consumers;

public class AppointmentRescheduledConsumer : IConsumer<AppointmentRescheduled>
{
    private readonly ILogger<AppointmentRescheduledConsumer> _logger;
    private readonly MedicalTreatmentDbContext _db;

    public AppointmentRescheduledConsumer(ILogger<AppointmentRescheduledConsumer> logger, MedicalTreatmentDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task Consume(ConsumeContext<AppointmentRescheduled> context)
    {
        var message = context.Message;
        var treatment = await _db.MedicalTreatments.FirstOrDefaultAsync(t => t.AppointmentId == message.AppointmentId);

        if (treatment is null)
        {
            _logger.LogInformation("Appointment rescheduled, but no treatment exists yet: {AppointmentId}", message.AppointmentId);
            return;
        }

        treatment.PatientId = message.PatientId;
        treatment.PhysicianId = message.PhysicianId;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Existing medical treatment reference updated after appointment reschedule: {AppointmentId}", message.AppointmentId);
    }
}
