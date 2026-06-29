using Data.Enums;

namespace Data.Models.MedicalTreatment;

public class MedicalTreatment
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? PhysicianId { get; set; }
    public required string Type { get; set; }
    public string? Results { get; set; }
    public TreatmentStatus Status { get; set; } = TreatmentStatus.Planned;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExaminedAt { get; set; }
}
