namespace Data.Contracts;

public record MedicationPrescribed
{
    public required Guid PrescriptionId { get; init; }
    public Guid? TreatmentId { get; init; }
    public Guid? AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public Guid? PhysicianId { get; init; }
    public Guid? PharmacyId { get; init; }
    public required string MedicationDetails { get; init; }
    public bool PharmacyNotified { get; init; }
    public DateTime PrescribedAt { get; init; } = DateTime.UtcNow;
}
