namespace Data.Contracts;

public record ProcedureRequested
{
    public required Guid ProcedureRequestId { get; init; }
    public required Guid TreatmentId { get; init; }
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public required string ProcedureType { get; init; }
    public required string Reason { get; init; }
    public DateTime RequestedAt { get; init; } = DateTime.UtcNow;
}
