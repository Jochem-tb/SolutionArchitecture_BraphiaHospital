namespace Data.Contracts;

public record LabResultAssociated
{
    public required Guid ProcedureRequestId { get; init; }
    public required Guid ProcedureResultId { get; init; }
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public Guid? GeneralPractitionerId { get; init; }
    public required string ResultSummary { get; init; }
    public DateTime AssociatedAt { get; init; } = DateTime.UtcNow;
}
