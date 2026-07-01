namespace PatientCare.Domain.Write;

public class MedicalHistoryEntry
{
    public Guid EntryId { get; set; }
    public Guid PatientId { get; set; }
    public Guid PhysicianId { get; set; }
    public required string Notes { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
