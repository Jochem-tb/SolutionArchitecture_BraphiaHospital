using Data.Contracts;
using MassTransit;
using PatientCare.Commands.Abstractions;
using PatientCare.Commands.CreatePrescription;

namespace PatientCare.Consumers;

public sealed class MedicationPrescribedConsumer : IConsumer<MedicationPrescribed>
{
    private readonly ICommandHandler<CreatePrescriptionCommand, Guid> _handler;

    public MedicationPrescribedConsumer(ICommandHandler<CreatePrescriptionCommand, Guid> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<MedicationPrescribed> context)
    {
        var message = context.Message;

        if (message.PatientId == Guid.Empty || string.IsNullOrWhiteSpace(message.MedicationDetails))
        {
            throw new ArgumentException("Invalid MedicationPrescribed payload.");
        }

        var command = new CreatePrescriptionCommand(
            message.PatientId,
            message.MedicationDetails,
            message.PharmacyNotified
        );

        await _handler.HandleAsync(command, context.CancellationToken);
    }
}
