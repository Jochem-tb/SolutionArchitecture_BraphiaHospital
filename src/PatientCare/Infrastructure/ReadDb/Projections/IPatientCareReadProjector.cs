using PatientCare.Contracts;

namespace PatientCare.Infrastructure.ReadDb.Projections;

public interface IPatientCareReadProjector
{
    Task Project_PatientCacheUpserted_Async(PatientCacheUpserted message, CancellationToken cancellationToken = default);
    Task Project_AppointmentCacheUpserted_Async(AppointmentCacheUpserted message, CancellationToken cancellationToken = default);
    Task Project_MedicalEntryWritten_Async(MedicalEntryWritten message, CancellationToken cancellationToken = default);
    Task Project_PrescriptionWritten_Async(PrescriptionWritten message, CancellationToken cancellationToken = default);
}
