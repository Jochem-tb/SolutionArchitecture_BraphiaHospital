namespace PatientCare.Contracts;

public sealed record AppointmentScheduledEvent(
    Guid AppointmentId,
    Guid PatientId,
    Guid PhysicianId,
    Guid ReferralId,
    Guid? GeneralPractitionerId,
    DateTime ScheduledAt,
    DateTime PlannedAt)
{
    public const string ContractName = "patientcare.events.appointment-scheduled";
    public const int Version = 1;
}
