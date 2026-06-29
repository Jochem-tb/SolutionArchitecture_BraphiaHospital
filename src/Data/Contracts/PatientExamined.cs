namespace Data.Contracts;

public record PatientExamined
{
    public required Guid TreatmentId { get; init; }
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public string? Diagnosis { get; init; }
    public DateTime ExaminedAt { get; init; } = DateTime.UtcNow;
}
