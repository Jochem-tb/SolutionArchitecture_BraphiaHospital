using PatientCare.Commands.Abstractions;
using PatientCare.Domain.Write;
using PatientCare.Infrastructure.WriteDb.Repositories;

namespace PatientCare.Commands.AddMedicalHistoryEntry;

public sealed class AddMedicalHistoryEntryCommandHandler
    : ICommandHandler<AddMedicalHistoryEntryCommand, Guid>
{
    private readonly IPatientCareWriteRepository _patientCareWriteRepository;

    public AddMedicalHistoryEntryCommandHandler(IPatientCareWriteRepository patientCareWriteRepository)
    {
        _patientCareWriteRepository = patientCareWriteRepository;
    }

    public async Task<Guid> HandleAsync(
        AddMedicalHistoryEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.PatientId == Guid.Empty)
        {
            throw new ArgumentException("PatientId is required.", nameof(command));
        }

        if (command.PhysicianId == Guid.Empty)
        {
            throw new ArgumentException("PhysicianId is required.", nameof(command));
        }

        if (string.IsNullOrWhiteSpace(command.Notes))
        {
            throw new ArgumentException("Notes are required.", nameof(command));
        }

        var entry = new MedicalHistoryEntry
        {
            EntryId = Guid.NewGuid(),
            PatientId = command.PatientId,
            PhysicianId = command.PhysicianId,
            Notes = command.Notes.Trim(),
            Timestamp = command.Timestamp ?? DateTime.UtcNow,
        };

        await _patientCareWriteRepository.AddMedicalEntry(entry, cancellationToken);
        return entry.EntryId;
    }
}
