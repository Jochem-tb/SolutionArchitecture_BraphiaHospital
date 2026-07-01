using Data.Contracts;
using MassTransit;
using PatientCare.Commands.Abstractions;
using PatientCare.Commands.AddMedicalHistoryEntry;

namespace PatientCare.Consumers;

public sealed class PatientExaminedConsumer : IConsumer<PatientExamined>
{
    private readonly ICommandHandler<AddMedicalHistoryEntryCommand, Guid> _handler;

    public PatientExaminedConsumer(ICommandHandler<AddMedicalHistoryEntryCommand, Guid> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<PatientExamined> context)
    {
        var message = context.Message;

        if (message.PatientId == Guid.Empty || message.PhysicianId == Guid.Empty)
        {
            throw new ArgumentException("Invalid PatientExamined payload.");
        }

        var notes = string.IsNullOrWhiteSpace(message.Diagnosis)
            ? "Patient examined"
            : message.Diagnosis;

        var command = new AddMedicalHistoryEntryCommand(
            message.PatientId,
            message.PhysicianId,
            notes,
            message.ExaminedAt
        );

        await _handler.HandleAsync(command, context.CancellationToken);
    }
}
