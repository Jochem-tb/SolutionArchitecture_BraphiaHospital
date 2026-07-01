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
}
