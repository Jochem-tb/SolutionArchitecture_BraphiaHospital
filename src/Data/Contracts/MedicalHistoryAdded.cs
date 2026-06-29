namespace Data.Contracts;

public record MedicalHistoryAdded
{
    public required Guid HistoryEntryId { get; init; }
    public Guid? TreatmentId { get; init; }
    public Guid? AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public string? EntryType { get; init; }
    public DateTime AddedAt { get; init; } = DateTime.UtcNow;
}
