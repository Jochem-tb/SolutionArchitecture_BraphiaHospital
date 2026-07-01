namespace PatientCare.Commands.UpsertPatientCache;

public sealed record UpsertPatientCacheCommand(
    Guid PatientId,
    string? Name
);
