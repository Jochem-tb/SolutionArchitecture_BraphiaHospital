using PatientCare.Commands.Abstractions;
using PatientCare.Domain.Write;
using PatientCare.Infrastructure.WriteDb.Repositories;

namespace PatientCare.Commands.CreatePrescription;

public sealed class CreatePrescriptionCommandHandler
    : ICommandHandler<CreatePrescriptionCommand, Guid>
{
    private readonly IPatientCareWriteRepository _patientCareWriteRepository;

    public CreatePrescriptionCommandHandler(IPatientCareWriteRepository patientCareWriteRepository)
    {
        _patientCareWriteRepository = patientCareWriteRepository;
    }

    public async Task<Guid> HandleAsync(
        CreatePrescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.PatientId == Guid.Empty)
        {
            throw new ArgumentException("PatientId is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.MedicationDetails))
        {
            throw new ArgumentException("MedicationDetails are required.", nameof(command));
        }

        var prescription = new Prescription
        {
            PrescriptionId = Guid.NewGuid(),
            PatientId = command.PatientId,
            MedicationDetails = command.MedicationDetails.Trim(),
            PharmacyNotified = command.PharmacyNotified,
        };

        await _patientCareWriteRepository.AddPrescription(prescription, cancellationToken);
        return prescription.PrescriptionId;
    }
}
