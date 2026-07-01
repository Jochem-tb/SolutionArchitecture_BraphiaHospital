using PatientCare.Domain.Write;

namespace PatientCare.Infrastructure.WriteDb.Repositories;

public class PatientCareWriteRepository : IPatientCareWriteRepository
{
    private readonly PatientCareWriteDbContext _dbContext;

    public PatientCareWriteRepository(PatientCareWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddMedicalEntry(MedicalHistoryEntry entry, CancellationToken cancellationToken = default)
    {
        await _dbContext.MedicalHistoryEntries.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddPrescription(Prescription prescription, CancellationToken cancellationToken = default)
    {
        await _dbContext.Prescriptions.AddAsync(prescription, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertPatientCache(PatientCache patient, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Patients.FindAsync(new object[] { patient.PatientId }, cancellationToken);
        if (existing is null)
        {
            await _dbContext.Patients.AddAsync(patient, cancellationToken);
        }
        else
        {
            existing.Name = patient.Name;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpsertAppointmentCache(AppointmentCache appointment, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Appointments.FindAsync(new object[] { appointment.AppointmentId }, cancellationToken);
        if (existing is null)
        {
            await _dbContext.Appointments.AddAsync(appointment, cancellationToken);
        }
        else
        {
            existing.PatientId = appointment.PatientId;
            existing.PhysicianId = appointment.PhysicianId;
            existing.ScheduledAt = appointment.ScheduledAt;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
