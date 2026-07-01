using Data.Contracts;
using MassTransit;
using PatientCare.Commands.Abstractions;
using PatientCare.Commands.UpsertPatientCache;

namespace PatientCare.Consumers;

public sealed class PatientRegisteredConsumer : IConsumer<PatientRegistered>
{
    private readonly ICommandHandler<UpsertPatientCacheCommand, Guid> _handler;

    public PatientRegisteredConsumer(ICommandHandler<UpsertPatientCacheCommand, Guid> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<PatientRegistered> context)
    {
        var message = context.Message;

        if (message.PatientId == Guid.Empty || string.IsNullOrWhiteSpace(message.Name))
        {
            throw new ArgumentException("Invalid PatientRegistered payload.");
        }

        var command = new UpsertPatientCacheCommand(
            message.PatientId,
            message.Name
        );

        await _handler.HandleAsync(command, context.CancellationToken);
    }
}
