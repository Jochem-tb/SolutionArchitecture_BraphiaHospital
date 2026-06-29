namespace Data.Contracts;

public record AppointmentRescheduled
{
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public required DateTime OldScheduledAt { get; init; }
    public required DateTime NewScheduledAt { get; init; }
    public DateTime RescheduledAt { get; init; } = DateTime.UtcNow;
}
