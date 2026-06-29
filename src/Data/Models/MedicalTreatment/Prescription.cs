namespace Data.Models.MedicalTreatment;

public class Prescription
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? TreatmentId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid? PhysicianId { get; set; }
    public required string MedicationDetails { get; set; }
    public bool PharmacyNotified { get; set; }
    public DateTime PrescribedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ValidUntil { get; set; }
}
