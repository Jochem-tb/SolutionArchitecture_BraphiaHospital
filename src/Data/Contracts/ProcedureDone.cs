namespace Data.Contracts;

public record ProcedureDone
{
    public required Guid ProcedureRequestId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid AppointmentId { get; init; }
    public required string ResultSummary { get; init; }
    public string? ResultDataJSON { get; init; }
    public DateTime PerformedAt { get; init; } = DateTime.UtcNow;
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
}
