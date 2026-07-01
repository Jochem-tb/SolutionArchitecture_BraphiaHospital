namespace PatientCare.Commands.AddMedicalHistoryEntry;

public sealed record AddMedicalHistoryEntryCommand(
    Guid PatientId,
    Guid PhysicianId,
    string Notes,
    DateTime? Timestamp = null
);
