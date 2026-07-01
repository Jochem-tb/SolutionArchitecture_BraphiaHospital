using PatientCare.Domain.Write;

namespace PatientCare.Infrastructure.WriteDb.Repositories;

public interface IPatientCareWriteRepository
{
    Task AddMedicalEntry(MedicalHistoryEntry entry, CancellationToken cancellationToken = default);
    Task AddPrescription(Prescription prescription, CancellationToken cancellationToken = default);
}
