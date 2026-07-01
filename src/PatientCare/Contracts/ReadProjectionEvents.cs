namespace PatientCare.Contracts;

public sealed record MedicalEntryWritten(
    Guid EntryId,
    Guid PatientId,
    Guid PhysicianId,
    string Notes,
    DateTime Timestamp
);

public sealed record PrescriptionWritten(
    Guid PrescriptionId,
    Guid PatientId,
    string MedicationDetails,
    bool PharmacyNotified
);

public sealed record PatientCacheUpserted(
    Guid PatientId,
    string? Name
);

public sealed record AppointmentCacheUpserted(
    Guid AppointmentId,
    Guid PatientId,
    Guid PhysicianId,
    DateTime? ScheduledAt
);
