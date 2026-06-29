namespace Data.Contracts;

public record AppointmentPlanned
{
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public required Guid ReferralId { get; init; }
    public Guid? GeneralPractitionerId { get; init; }
    public required DateTime ScheduledAt { get; init; }
    public DateTime PlannedAt { get; init; } = DateTime.UtcNow;
}
