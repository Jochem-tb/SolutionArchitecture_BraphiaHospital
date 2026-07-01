namespace PatientCare.Contracts;

public sealed record MedicationPrescribedEvent(
    Guid PrescriptionId,
    Guid? TreatmentId,
    Guid? AppointmentId,
    Guid PatientId,
    Guid? PhysicianId,
    Guid? PharmacyId,
    string MedicationDetails,
    bool PharmacyNotified,
    DateTime PrescribedAt)
{
    public const string ContractName = "patientcare.events.medication-prescribed";
    public const int Version = 1;
}
