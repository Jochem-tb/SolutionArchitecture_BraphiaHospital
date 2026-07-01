using Microsoft.EntityFrameworkCore;
using PatientCare.Infrastructure.ReadDb;

namespace PatientCare.Queries.GetMedicalDossier;

public sealed record GetMedicalDossierQuery(Guid PatientId);

public sealed class GetMedicalDossierResult
{
	public Guid PatientId { get; init; }
	public string? PatientName { get; init; }
	public DateTime LastUpdatedAt { get; init; }

	public IReadOnlyList<MedicalHistoryItemDto> MedicalHistory { get; init; } = Array.Empty<MedicalHistoryItemDto>();
	public IReadOnlyList<ActivePrescriptionItemDto> ActivePrescriptions { get; init; } = Array.Empty<ActivePrescriptionItemDto>();
	public IReadOnlyList<UpcomingAppointmentItemDto> UpcomingAppointments { get; init; } = Array.Empty<UpcomingAppointmentItemDto>();
}

public sealed class MedicalHistoryItemDto
{
	public Guid EntryId { get; init; }
	public Guid PhysicianId { get; init; }
	public string Notes { get; init; } = string.Empty;
	public DateTime Timestamp { get; init; }
}

public sealed class ActivePrescriptionItemDto
{
	public Guid PrescriptionId { get; init; }
	public string MedicationDetails { get; init; } = string.Empty;
	public bool PharmacyNotified { get; init; }
}

public sealed class UpcomingAppointmentItemDto
{
	public Guid AppointmentId { get; init; }
	public Guid PhysicianId { get; init; }
	public DateTime? ScheduledAt { get; init; }
}

public sealed class GetMedicalDossierQueryHandler
{
	private readonly PatientCareReadDbContext _readDbContext;

	public GetMedicalDossierQueryHandler(PatientCareReadDbContext readDbContext)
	{
		_readDbContext = readDbContext;
	}

	public async Task<GetMedicalDossierResult?> HandleAsync(
		GetMedicalDossierQuery query,
		CancellationToken cancellationToken = default)
	{
		if (query.PatientId == Guid.Empty)
		{
			throw new ArgumentException("PatientId is required.", nameof(query));
		}

		var dossier = await _readDbContext.PatientDossiers
			.AsNoTracking()
			.Include(x => x.MedicalHistory)
			.Include(x => x.ActivePrescriptions)
			.Include(x => x.UpcomingAppointments)
			.FirstOrDefaultAsync(x => x.PatientId == query.PatientId, cancellationToken);

		if (dossier is null)
		{
			return null;
		}

		return new GetMedicalDossierResult
		{
			PatientId = dossier.PatientId,
			PatientName = dossier.PatientName,
			LastUpdatedAt = dossier.LastUpdatedAt,
			MedicalHistory = dossier.MedicalHistory
				.OrderByDescending(x => x.Timestamp)
				.Select(x => new MedicalHistoryItemDto
				{
					EntryId = x.Id,
					PhysicianId = x.PhysicianId,
					Notes = x.Notes,
					Timestamp = x.Timestamp,
				})
				.ToList(),
			ActivePrescriptions = dossier.ActivePrescriptions
				.Select(x => new ActivePrescriptionItemDto
				{
					PrescriptionId = x.Id,
					MedicationDetails = x.MedicationDetails,
					PharmacyNotified = x.PharmacyNotified,
				})
				.ToList(),
			UpcomingAppointments = dossier.UpcomingAppointments
				.OrderBy(x => x.ScheduledAt)
				.Select(x => new UpcomingAppointmentItemDto
				{
					AppointmentId = x.AppointmentId,
					PhysicianId = x.PhysicianId,
					ScheduledAt = x.ScheduledAt,
				})
				.ToList(),
		};
	}
}
