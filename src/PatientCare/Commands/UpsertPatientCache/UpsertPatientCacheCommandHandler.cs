using PatientCare.Commands.Abstractions;
using MassTransit;
using PatientCare.Contracts;
using PatientCare.Domain.Write;
using PatientCare.Infrastructure.WriteDb.Repositories;

namespace PatientCare.Commands.UpsertPatientCache;

public sealed class UpsertPatientCacheCommandHandler
    : ICommandHandler<UpsertPatientCacheCommand, Guid>
{
    private readonly IPatientCareWriteRepository _patientCareWriteRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpsertPatientCacheCommandHandler(
        IPatientCareWriteRepository patientCareWriteRepository,
        IPublishEndpoint publishEndpoint)
    {
        _patientCareWriteRepository = patientCareWriteRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> HandleAsync(
        UpsertPatientCacheCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.PatientId == Guid.Empty)
        {
            throw new ArgumentException("PatientId is required.", nameof(command));
        }

        var patient = new PatientCache
        {
            PatientId = command.PatientId,
            Name = string.IsNullOrWhiteSpace(command.Name) ? null : command.Name.Trim(),
        };

        await _patientCareWriteRepository.UpsertPatientCache(patient, cancellationToken);
        await _publishEndpoint.Publish(
            new PatientCacheUpserted(patient.PatientId, patient.Name),
            cancellationToken);

        return patient.PatientId;
    }
}
