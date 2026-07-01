using Data.Enums;

namespace Data.Contracts;

public record PatientCheckedIn
{
    public static string StreamName = "Appointment";
    public required Guid AppointmentId { get; init; }
    public required Guid PatientId { get; init; }
    public required Guid PhysicianId { get; init; }
    public required AppointmentArrivalStatus ArrivalStatus { get; init; }
    public DateTime CheckedInAt { get; init; } = DateTime.UtcNow;
}
