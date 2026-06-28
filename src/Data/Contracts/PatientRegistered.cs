namespace Data.Contracts;


public record PatientRegistered
{
    public required Guid PatientId { get; init; }
    public required string Name { get; init; }
    public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;
}