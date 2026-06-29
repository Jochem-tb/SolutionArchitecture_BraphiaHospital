namespace Data.Models.MedicalTreatment;

public class MedicalHistoryEntry
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? PhysicianId { get; set; }
    public Guid? TreatmentId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string? EntryType { get; set; }
    public required string Notes { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
