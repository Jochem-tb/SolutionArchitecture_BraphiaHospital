namespace Data.Contracts;

public record PatientRegistered
{
    public required Guid PatientId { get; init; }
    public required string Name { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? ContactDetails { get; init; }
    public string? InsuranceNumber { get; init; }
    public bool IsIdentityVerified { get; init; }
    public Guid? GeneralPractitionerId { get; init; }
    public DateTime RegisteredAt { get; init; } = DateTime.UtcNow;
}
