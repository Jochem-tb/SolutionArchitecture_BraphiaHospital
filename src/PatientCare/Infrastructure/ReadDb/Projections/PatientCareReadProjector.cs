using Microsoft.EntityFrameworkCore;
using PatientCare.Contracts;
using PatientCare.Domain.Read;

namespace PatientCare.Infrastructure.ReadDb.Projections;

public sealed class PatientCareReadProjector : IPatientCareReadProjector
{
    private readonly PatientCareReadDbContext _readDbContext;

    public PatientCareReadProjector(PatientCareReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task Project_PatientCacheUpserted_Async(PatientCacheUpserted message, CancellationToken cancellationToken = default)
    {
        var dossier = await EnsureDossierAsync(message.PatientId, cancellationToken);
        dossier.PatientName = message.Name;
        dossier.LastUpdatedAt = DateTime.UtcNow;
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Project_AppointmentCacheUpserted_Async(AppointmentCacheUpserted message, CancellationToken cancellationToken = default)
    {
        var dossier = await EnsureDossierAsync(message.PatientId, cancellationToken);

        var existing = await _readDbContext.UpcomingAppointmentItems
            .FirstOrDefaultAsync(x => x.AppointmentId == message.AppointmentId, cancellationToken);

        if (existing is null)
        {
            await _readDbContext.UpcomingAppointmentItems.AddAsync(new AppointmentReadItem
            {
                Id = Guid.NewGuid(),
                PatientId = message.PatientId,
                AppointmentId = message.AppointmentId,
                PhysicianId = message.PhysicianId,
                ScheduledAt = message.ScheduledAt,
            }, cancellationToken);
        }
        else
        {
            existing.PatientId = message.PatientId;
            existing.PhysicianId = message.PhysicianId;
            existing.ScheduledAt = message.ScheduledAt;
        }

        dossier.LastUpdatedAt = DateTime.UtcNow;
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Project_MedicalEntryWritten_Async(MedicalEntryWritten message, CancellationToken cancellationToken = default)
    {
        var dossier = await EnsureDossierAsync(message.PatientId, cancellationToken);

        var exists = await _readDbContext.MedicalHistoryItems
            .AnyAsync(x => x.Id == message.EntryId, cancellationToken);

        if (!exists)
        {
            await _readDbContext.MedicalHistoryItems.AddAsync(new MedicalHistoryReadItem
            {
                Id = message.EntryId,
                PatientId = message.PatientId,
                PhysicianId = message.PhysicianId,
                Notes = message.Notes,
                Timestamp = message.Timestamp,
            }, cancellationToken);
        }

        dossier.LastUpdatedAt = DateTime.UtcNow;
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Project_PrescriptionWritten_Async(PrescriptionWritten message, CancellationToken cancellationToken = default)
    {
        var dossier = await EnsureDossierAsync(message.PatientId, cancellationToken);

        var exists = await _readDbContext.ActivePrescriptionItems
            .AnyAsync(x => x.Id == message.PrescriptionId, cancellationToken);

        if (!exists)
        {
            await _readDbContext.ActivePrescriptionItems.AddAsync(new PrescriptionReadItem
            {
                Id = message.PrescriptionId,
                PatientId = message.PatientId,
                MedicationDetails = message.MedicationDetails,
                PharmacyNotified = message.PharmacyNotified,
            }, cancellationToken);
        }

        dossier.LastUpdatedAt = DateTime.UtcNow;
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<PatientDossierReadModel> EnsureDossierAsync(Guid patientId, CancellationToken cancellationToken)
    {
        var existing = await _readDbContext.PatientDossiers
            .FirstOrDefaultAsync(x => x.PatientId == patientId, cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var dossier = new PatientDossierReadModel
        {
            PatientId = patientId,
            LastUpdatedAt = DateTime.UtcNow,
            PatientName = string.Empty,
        };

        await _readDbContext.PatientDossiers.AddAsync(dossier, cancellationToken);
        await _readDbContext.SaveChangesAsync(cancellationToken);

        return dossier;
    }
}
