namespace PatientCare.Contracts;

public sealed record PatientRegisteredEvent(
    Guid PatientId,
    string Name,
    string? FirstName,
    string? LastName,
    string? ContactDetails,
    string? InsuranceNumber,
    bool IsIdentityVerified,
    Guid? GeneralPractitionerId,
    DateTime RegisteredAt)
{
    public const string ContractName = "patientcare.events.patient-registered";
    public const int Version = 1;
}
