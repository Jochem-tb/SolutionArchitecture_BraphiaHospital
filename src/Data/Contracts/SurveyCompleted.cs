namespace Data.Contracts;

public record SurveyCompleted
{
    public required Guid SurveyId { get; init; }
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required string HealthDataJSON { get; init; }
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
}
