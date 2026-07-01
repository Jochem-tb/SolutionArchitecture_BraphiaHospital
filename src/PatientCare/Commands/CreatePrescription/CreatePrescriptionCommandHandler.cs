using PatientCare.Commands.Abstractions;
using MassTransit;
using PatientCare.Contracts;
using PatientCare.Domain.Write;
using PatientCare.Infrastructure.WriteDb.Repositories;

namespace PatientCare.Commands.CreatePrescription;

public sealed class CreatePrescriptionCommandHandler
    : ICommandHandler<CreatePrescriptionCommand, Guid>
{
    private readonly IPatientCareWriteRepository _patientCareWriteRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreatePrescriptionCommandHandler(
        IPatientCareWriteRepository patientCareWriteRepository,
        IPublishEndpoint publishEndpoint)
    {
        _patientCareWriteRepository = patientCareWriteRepository;
        _publishEndpoint = publishEndpoint;
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
        await _publishEndpoint.Publish(
            new PrescriptionWritten(
                prescription.PrescriptionId,
                prescription.PatientId,
                prescription.MedicationDetails,
                prescription.PharmacyNotified),
            cancellationToken);

        return prescription.PrescriptionId;
    }
}
