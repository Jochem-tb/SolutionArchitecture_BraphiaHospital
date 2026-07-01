namespace PatientCare.Domain.Read;

public class PatientDossierReadModel
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MedicalHistoryReadItem> MedicalHistory { get; set; } = new List<MedicalHistoryReadItem>();
    public ICollection<PrescriptionReadItem> ActivePrescriptions { get; set; } = new List<PrescriptionReadItem>();
    public ICollection<AppointmentReadItem> UpcomingAppointments { get; set; } = new List<AppointmentReadItem>();
}

public class MedicalHistoryReadItem
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid PhysicianId { get; set; }
    public required string Notes { get; set; }
    public DateTime Timestamp { get; set; }
}

public class PrescriptionReadItem
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public required string MedicationDetails { get; set; }
    public bool PharmacyNotified { get; set; }
}

public class AppointmentReadItem
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PhysicianId { get; set; }
    public DateTime? ScheduledAt { get; set; }
}
