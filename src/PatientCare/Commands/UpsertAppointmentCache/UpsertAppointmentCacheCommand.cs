namespace PatientCare.Commands.UpsertAppointmentCache;

public sealed record UpsertAppointmentCacheCommand(
    Guid AppointmentId,
    Guid PatientId,
    Guid PhysicianId,
    DateTime? ScheduledAt
);
