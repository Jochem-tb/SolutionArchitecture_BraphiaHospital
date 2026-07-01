namespace PatientCare.Domain.Write;

public class AppointmentCache
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid PhysicianId { get; set; }
    public DateTime? ScheduledAt { get; set; }
}
