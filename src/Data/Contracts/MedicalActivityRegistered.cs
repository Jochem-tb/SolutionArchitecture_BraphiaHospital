namespace Data.Contracts;

public record MedicalActivityRegistered
{
    public required Guid ActivityId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid AppointmentId { get; init; }
    public Guid? TreatmentId { get; init; }
    public required string ActivityType { get; init; }
    public required string BillingCode { get; init; }
    public string? Description { get; init; }
    public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;
}
