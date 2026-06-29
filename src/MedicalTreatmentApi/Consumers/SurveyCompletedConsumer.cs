using Data.Contracts;
using Data.DbContexts;
using Data.Models.MedicalTreatment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MedicalTreatmentApi.Consumers;

public class SurveyCompletedConsumer : IConsumer<SurveyCompleted>
{
    private readonly ILogger<SurveyCompletedConsumer> _logger;
    private readonly MedicalTreatmentDbContext _db;

    public SurveyCompletedConsumer(ILogger<SurveyCompletedConsumer> logger, MedicalTreatmentDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task Consume(ConsumeContext<SurveyCompleted> context)
    {
        var message = context.Message;

        var entry = await _db.MedicalHistoryEntries.FirstOrDefaultAsync(h => h.Id == message.SurveyId);
        if (entry is null)
        {
            entry = new MedicalHistoryEntry
            {
                Id = message.SurveyId,
                PatientId = message.PatientId,
                AppointmentId = message.AppointmentId,
                EntryType = "HealthQuestionnaire",
                Notes = message.HealthDataJSON,
                Timestamp = message.CompletedAt
            };
            _db.MedicalHistoryEntries.Add(entry);
        }
        else
        {
            entry.PatientId = message.PatientId;
            entry.AppointmentId = message.AppointmentId;
            entry.EntryType = "HealthQuestionnaire";
            entry.Notes = message.HealthDataJSON;
            entry.Timestamp = message.CompletedAt;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Completed questionnaire stored as medical history entry for patient: {PatientId}", message.PatientId);
    }
}
