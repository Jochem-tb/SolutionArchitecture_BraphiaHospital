using PatientCare.Commands.Abstractions;
using MassTransit;
using PatientCare.Contracts;
using PatientCare.Domain.Write;
using PatientCare.Infrastructure.WriteDb.Repositories;

namespace PatientCare.Commands.UpsertAppointmentCache;

public sealed class UpsertAppointmentCacheCommandHandler
    : ICommandHandler<UpsertAppointmentCacheCommand, Guid>
{
    private readonly IPatientCareWriteRepository _patientCareWriteRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpsertAppointmentCacheCommandHandler(
        IPatientCareWriteRepository patientCareWriteRepository,
        IPublishEndpoint publishEndpoint)
    {
        _patientCareWriteRepository = patientCareWriteRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> HandleAsync(
        UpsertAppointmentCacheCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AppointmentId == Guid.Empty)
        {
            throw new ArgumentException("AppointmentId is required.", nameof(command));
        }

        if (command.PatientId == Guid.Empty)
        {
            throw new ArgumentException("PatientId is required.", nameof(command));
        }

        if (command.PhysicianId == Guid.Empty)
        {
            throw new ArgumentException("PhysicianId is required.", nameof(command));
        }

        var appointment = new AppointmentCache
        {
            AppointmentId = command.AppointmentId,
            PatientId = command.PatientId,
            PhysicianId = command.PhysicianId,
            ScheduledAt = command.ScheduledAt,
        };

        await _patientCareWriteRepository.UpsertAppointmentCache(appointment, cancellationToken);
        await _publishEndpoint.Publish(
            new AppointmentCacheUpserted(
                appointment.AppointmentId,
                appointment.PatientId,
                appointment.PhysicianId,
                appointment.ScheduledAt),
            cancellationToken);

        return appointment.AppointmentId;
    }
}
