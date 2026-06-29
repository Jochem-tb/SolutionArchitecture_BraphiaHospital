using Data.Contracts;
using Data.DbContexts;
using Data.Enums;
using Data.Models.MedicalTreatment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MedicalTreatmentApi.Consumers;

public class ProcedureDoneConsumer : IConsumer<ProcedureDone>
{
    private readonly ILogger<ProcedureDoneConsumer> _logger;
    private readonly MedicalTreatmentDbContext _db;
    private readonly IPublishEndpoint _publishEndpoint;

    public ProcedureDoneConsumer(ILogger<ProcedureDoneConsumer> logger, MedicalTreatmentDbContext db, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _db = db;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<ProcedureDone> context)
    {
        var message = context.Message;

        var treatment = await _db.MedicalTreatments
            .FirstOrDefaultAsync(t => t.AppointmentId == message.AppointmentId && t.PatientId == message.PatientId);

        var notes = string.IsNullOrWhiteSpace(message.ResultDataJSON)
            ? message.ResultSummary
            : $"{message.ResultSummary}\n\nRaw result data: {message.ResultDataJSON}";

        var historyEntry = new MedicalHistoryEntry
        {
            Id = Guid.NewGuid(),
            TreatmentId = treatment?.Id,
            PatientId = message.PatientId,
            AppointmentId = message.AppointmentId,
            PhysicianId = treatment?.PhysicianId,
            EntryType = "ProcedureResult",
            Notes = notes,
            Timestamp = message.CompletedAt
        };

        if (treatment is not null)
        {
            treatment.Results = message.ResultSummary;
            treatment.Status = TreatmentStatus.ResultsAvailable;
        }

        _db.MedicalHistoryEntries.Add(historyEntry);
        await _db.SaveChangesAsync();

        await _publishEndpoint.Publish(new LabResultAssociated
        {
            ProcedureRequestId = message.ProcedureRequestId,
            ProcedureResultId = historyEntry.Id,
            AppointmentId = message.AppointmentId,
            PatientId = message.PatientId,
            PhysicianId = treatment?.PhysicianId ?? Guid.Empty,
            GeneralPractitionerId = null,
            ResultSummary = message.ResultSummary,
            AssociatedAt = historyEntry.Timestamp
        });

        await _publishEndpoint.Publish(new MedicalHistoryAdded
        {
            HistoryEntryId = historyEntry.Id,
            TreatmentId = historyEntry.TreatmentId,
            AppointmentId = historyEntry.AppointmentId,
            PatientId = historyEntry.PatientId,
            PhysicianId = historyEntry.PhysicianId ?? Guid.Empty,
            EntryType = historyEntry.EntryType,
            AddedAt = historyEntry.Timestamp
        });

        await _publishEndpoint.Publish(new MedicalActivityRegistered
        {
            ActivityId = Guid.NewGuid(),
            PatientId = message.PatientId,
            AppointmentId = message.AppointmentId,
            TreatmentId = treatment?.Id,
            ActivityType = "ProcedureDone",
            BillingCode = "LAB_RESULT",
            Description = message.ResultSummary,
            RegisteredAt = historyEntry.Timestamp
        });

        _logger.LogInformation("Procedure result stored as medical history entry for patient: {PatientId}", message.PatientId);
    }
}
