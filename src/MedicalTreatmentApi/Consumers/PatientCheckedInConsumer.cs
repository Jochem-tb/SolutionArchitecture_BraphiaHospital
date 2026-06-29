using Data.Contracts;
using Data.DbContexts;
using Data.Enums;
using Data.Models.MedicalTreatment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MedicalTreatmentApi.Consumers;

public class PatientCheckedInConsumer : IConsumer<PatientCheckedIn>
{
    private readonly ILogger<PatientCheckedInConsumer> _logger;
    private readonly MedicalTreatmentDbContext _db;

    public PatientCheckedInConsumer(ILogger<PatientCheckedInConsumer> logger, MedicalTreatmentDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task Consume(ConsumeContext<PatientCheckedIn> context)
    {
        var message = context.Message;

        var treatment = await _db.MedicalTreatments.FirstOrDefaultAsync(t => t.AppointmentId == message.AppointmentId);
        if (treatment is null)
        {
            treatment = new MedicalTreatment
            {
                Id = Guid.NewGuid(),
                AppointmentId = message.AppointmentId,
                PatientId = message.PatientId,
                PhysicianId = message.PhysicianId,
                Type = "Consult",
                CreatedAt = DateTime.UtcNow,
                Status = TreatmentStatus.PatientArrived
            };
            _db.MedicalTreatments.Add(treatment);
        }
        else
        {
            treatment.PatientId = message.PatientId;
            treatment.PhysicianId = message.PhysicianId;
            treatment.Status = TreatmentStatus.PatientArrived;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Treatment prepared after patient check-in: {AppointmentId}", message.AppointmentId);
    }
}
