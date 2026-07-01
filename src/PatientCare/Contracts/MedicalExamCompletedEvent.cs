namespace PatientCare.Contracts;

public sealed record MedicalExamCompletedEvent(
    Guid TreatmentId,
    Guid AppointmentId,
    Guid PatientId,
    Guid PhysicianId,
    string? Diagnosis,
    DateTime ExaminedAt)
{
    public const string ContractName = "patientcare.events.medical-exam-completed";
    public const int Version = 1;
}
