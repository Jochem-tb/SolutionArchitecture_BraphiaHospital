namespace Data.Contracts;

public record PatientCheckedIn
{
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public required string ArrivalStatus { get; init; }
    public DateTime CheckedInAt { get; init; } = DateTime.UtcNow;
}
