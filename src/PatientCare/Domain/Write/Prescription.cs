namespace PatientCare.Domain.Write;

public class Prescription
{
    public Guid PrescriptionId { get; set; }
    public Guid PatientId { get; set; }
    public required string MedicationDetails { get; set; }
    public bool PharmacyNotified { get; set; }
}
