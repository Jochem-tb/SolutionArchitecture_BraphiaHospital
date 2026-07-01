namespace PatientCare.Commands.CreatePrescription;

public sealed record CreatePrescriptionCommand(
    Guid PatientId,
    string MedicationDetails,
    bool PharmacyNotified = false
);
