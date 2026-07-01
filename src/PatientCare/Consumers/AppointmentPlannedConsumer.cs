using Data.Contracts;
using MassTransit;
using PatientCare.Commands.Abstractions;
using PatientCare.Commands.UpsertAppointmentCache;

namespace PatientCare.Consumers;

public sealed class AppointmentPlannedConsumer : IConsumer<AppointmentPlanned>
{
    private readonly ICommandHandler<UpsertAppointmentCacheCommand, Guid> _handler;

    public AppointmentPlannedConsumer(ICommandHandler<UpsertAppointmentCacheCommand, Guid> handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<AppointmentPlanned> context)
    {
        var message = context.Message;

        if (message.AppointmentId == Guid.Empty || message.PatientId == Guid.Empty || message.PhysicianId == Guid.Empty)
        {
            throw new ArgumentException("Invalid AppointmentPlanned payload.");
        }

        var command = new UpsertAppointmentCacheCommand(
            message.AppointmentId,
            message.PatientId,
            message.PhysicianId,
            message.ScheduledAt
        );

        await _handler.HandleAsync(command, context.CancellationToken);
    }
}
