using Data.Contracts;
using Data.DbContexts;
using Data.Enums;
using Data.Models.MedicalTreatment;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MedicalTreatmentApi;

public static class MedicalTreatmentEndpoints
{
    public static WebApplication MapMedicalTreatmentEndpoints(this WebApplication app)
    {
        app.MapGet(
                "/medical-treatments/{treatmentId:guid}",
                async (Guid treatmentId, MedicalTreatmentDbContext db) =>
                {
                    var treatment = await db.MedicalTreatments.FirstOrDefaultAsync(t =>
                        t.Id == treatmentId
                    );
                    if (treatment is null)
                    {
                        return Results.NotFound("Treatment not found.");
                    }

                    var history = await db
                        .MedicalHistoryEntries.Where(h => h.TreatmentId == treatmentId)
                        .OrderByDescending(h => h.Timestamp)
                        .ToListAsync();

                    var prescriptions = await db
                        .Prescriptions.Where(p => p.TreatmentId == treatmentId)
                        .OrderByDescending(p => p.PrescribedAt)
                        .ToListAsync();

                    return Results.Ok(
                        new
                        {
                            Treatment = treatment,
                            MedicalHistory = history,
                            Prescriptions = prescriptions,
                        }
                    );
                }
            )
            .WithName("GetMedicalTreatment");

        app.MapGet(
                "/medical-treatments/patients/{patientId:guid}/history",
                async (Guid patientId, MedicalTreatmentDbContext db) =>
                {
                    var history = await db
                        .MedicalHistoryEntries.Where(h => h.PatientId == patientId)
                        .OrderByDescending(h => h.Timestamp)
                        .ToListAsync();

                    var treatments = await db
                        .MedicalTreatments.Where(t => t.PatientId == patientId)
                        .OrderByDescending(t => t.CreatedAt)
                        .ToListAsync();

                    var prescriptions = await db
                        .Prescriptions.Where(p => p.PatientId == patientId)
                        .OrderByDescending(p => p.PrescribedAt)
                        .ToListAsync();

                    return Results.Ok(
                        new
                        {
                            PatientId = patientId,
                            MedicalHistory = history,
                            Treatments = treatments,
                            Prescriptions = prescriptions,
                        }
                    );
                }
            )
            .WithName("GetPatientMedicalHistory");

        app.MapPost(
                "/medical-treatments/exams",
                async (
                    CreatePatientExamRequest request,
                    MedicalTreatmentDbContext db,
                    IPublishEndpoint publishEndpoint
                ) =>
                {
                    if (request.PatientId == Guid.Empty)
                    {
                        return Results.BadRequest("PatientId is required.");
                    }

                    if (request.PhysicianId == Guid.Empty)
                    {
                        return Results.BadRequest("PhysicianId is required.");
                    }

                    Guid? appointmentId =
                        request.AppointmentId is null || request.AppointmentId.Value == Guid.Empty
                            ? null
                            : request.AppointmentId;
                    var treatment = appointmentId is null
                        ? null
                        : await db.MedicalTreatments.FirstOrDefaultAsync(t =>
                            t.AppointmentId == appointmentId.Value
                        );

                    if (treatment is null)
                    {
                        treatment = new MedicalTreatment
                        {
                            Id = Guid.NewGuid(),
                            AppointmentId = appointmentId,
                            PatientId = request.PatientId,
                            PhysicianId = request.PhysicianId,
                            Type = string.IsNullOrWhiteSpace(request.TreatmentType)
                                ? "Consult"
                                : request.TreatmentType,
                            CreatedAt = DateTime.UtcNow,
                        };

                        db.MedicalTreatments.Add(treatment);
                    }

                    treatment.PatientId = request.PatientId;
                    treatment.PhysicianId = request.PhysicianId;
                    treatment.Type = string.IsNullOrWhiteSpace(request.TreatmentType)
                        ? treatment.Type
                        : request.TreatmentType!;
                    treatment.Status = TreatmentStatus.Examined;
                    treatment.Results = !string.IsNullOrWhiteSpace(request.Results)
                        ? request.Results
                        : request.Diagnosis;
                    treatment.ExaminedAt = DateTime.UtcNow;

                    var historyEntry = new MedicalHistoryEntry
                    {
                        Id = Guid.NewGuid(),
                        TreatmentId = treatment.Id,
                        PatientId = request.PatientId,
                        AppointmentId = appointmentId,
                        PhysicianId = request.PhysicianId,
                        EntryType = "Exam",
                        Notes = request.Notes,
                        Timestamp = DateTime.UtcNow,
                    };

                    db.MedicalHistoryEntries.Add(historyEntry);
                    await db.SaveChangesAsync();

                    await publishEndpoint.Publish(
                        new PatientExamined
                        {
                            TreatmentId = treatment.Id,
                            AppointmentId = treatment.AppointmentId ?? Guid.Empty,
                            PatientId = treatment.PatientId,
                            PhysicianId = treatment.PhysicianId ?? request.PhysicianId,
                            Diagnosis = treatment.Results,
                            ExaminedAt = treatment.ExaminedAt.Value,
                        }
                    );

                    await publishEndpoint.Publish(
                        new MedicalHistoryAdded
                        {
                            HistoryEntryId = historyEntry.Id,
                            TreatmentId = treatment.Id,
                            AppointmentId = treatment.AppointmentId,
                            PatientId = treatment.PatientId,
                            PhysicianId = historyEntry.PhysicianId ?? Guid.Empty,
                            EntryType = historyEntry.EntryType,
                            AddedAt = historyEntry.Timestamp,
                        }
                    );

                    await PublishMedicalActivityAsync(
                        publishEndpoint,
                        treatment,
                        "PatientExamined",
                        "CONSULT",
                        "Patient examination completed."
                    );

                    return Results.Created(
                        $"/medical-treatments/{treatment.Id}",
                        new { TreatmentId = treatment.Id, HistoryEntryId = historyEntry.Id }
                    );
                }
            )
            .WithName("CreatePatientExam");

        app.MapPost(
                "/medical-treatments/{treatmentId:guid}/notes",
                async (
                    Guid treatmentId,
                    AddMedicalHistoryNoteRequest request,
                    MedicalTreatmentDbContext db,
                    IPublishEndpoint publishEndpoint
                ) =>
                {
                    var treatment = await db.MedicalTreatments.FindAsync(treatmentId);
                    if (treatment is null)
                    {
                        return Results.NotFound("Treatment not found.");
                    }

                    var physicianId = request.PhysicianId ?? treatment.PhysicianId;
                    if (physicianId is null || physicianId == Guid.Empty)
                    {
                        return Results.BadRequest(
                            "PhysicianId is required for a medical history note."
                        );
                    }

                    var historyEntry = new MedicalHistoryEntry
                    {
                        Id = Guid.NewGuid(),
                        TreatmentId = treatment.Id,
                        PatientId = treatment.PatientId,
                        AppointmentId = treatment.AppointmentId,
                        PhysicianId = physicianId.Value,
                        EntryType = request.EntryType,
                        Notes = request.Notes,
                        Timestamp = DateTime.UtcNow,
                    };

                    db.MedicalHistoryEntries.Add(historyEntry);
                    await db.SaveChangesAsync();

                    await publishEndpoint.Publish(
                        new MedicalHistoryAdded
                        {
                            HistoryEntryId = historyEntry.Id,
                            TreatmentId = treatment.Id,
                            AppointmentId = treatment.AppointmentId,
                            PatientId = treatment.PatientId,
                            PhysicianId = historyEntry.PhysicianId ?? Guid.Empty,
                            EntryType = historyEntry.EntryType,
                            AddedAt = historyEntry.Timestamp,
                        }
                    );

                    await PublishMedicalActivityAsync(
                        publishEndpoint,
                        treatment,
                        "MedicalHistoryAdded",
                        "DOSSIER_NOTE",
                        $"Medical history note added: {request.EntryType}."
                    );

                    return Results.Created(
                        $"/medical-treatments/{treatment.Id}/notes/{historyEntry.Id}",
                        new { historyEntry.Id }
                    );
                }
            )
            .WithName("AddMedicalHistoryNote");

        app.MapPost(
                "/medical-treatments/{treatmentId:guid}/prescriptions",
                async (
                    Guid treatmentId,
                    PrescribeMedicationRequest request,
                    MedicalTreatmentDbContext db,
                    IPublishEndpoint publishEndpoint
                ) =>
                {
                    var treatment = await db.MedicalTreatments.FindAsync(treatmentId);
                    if (treatment is null)
                    {
                        return Results.NotFound("Treatment not found.");
                    }

                    var medicationDetails = BuildMedicationDetails(request);
                    if (string.IsNullOrWhiteSpace(medicationDetails))
                    {
                        return Results.BadRequest("MedicationDetails is required.");
                    }

                    var prescription = new Prescription
                    {
                        Id = Guid.NewGuid(),
                        TreatmentId = treatment.Id,
                        PatientId = treatment.PatientId,
                        AppointmentId = treatment.AppointmentId,
                        PhysicianId = treatment.PhysicianId,
                        MedicationDetails = medicationDetails,
                        PharmacyNotified = false,
                        PrescribedAt = DateTime.UtcNow,
                        ValidUntil = request.ValidUntil,
                    };

                    db.Prescriptions.Add(prescription);
                    await db.SaveChangesAsync();

                    await publishEndpoint.Publish(
                        new MedicationPrescribed
                        {
                            PrescriptionId = prescription.Id,
                            TreatmentId = treatment.Id,
                            AppointmentId = treatment.AppointmentId,
                            PatientId = treatment.PatientId,
                            PhysicianId = treatment.PhysicianId,
                            PharmacyId = null,
                            MedicationDetails = prescription.MedicationDetails,
                            PharmacyNotified = true,
                            PrescribedAt = prescription.PrescribedAt,
                        }
                    );

                    prescription.PharmacyNotified = true;
                    await db.SaveChangesAsync();

                    await PublishMedicalActivityAsync(
                        publishEndpoint,
                        treatment,
                        "MedicationPrescribed",
                        "PRESCRIPTION",
                        prescription.MedicationDetails
                    );

                    return Results.Created(
                        $"/medical-treatments/{treatment.Id}/prescriptions/{prescription.Id}",
                        new { prescription.Id }
                    );
                }
            )
            .WithName("PrescribeMedication");

        app.MapPost(
                "/medical-treatments/{treatmentId:guid}/procedure-requests",
                async (
                    Guid treatmentId,
                    RequestProcedureRequest request,
                    MedicalTreatmentDbContext db,
                    IPublishEndpoint publishEndpoint
                ) =>
                {
                    var treatment = await db.MedicalTreatments.FindAsync(treatmentId);
                    if (treatment is null)
                    {
                        return Results.NotFound("Treatment not found.");
                    }

                    if (string.IsNullOrWhiteSpace(request.ProcedureType))
                    {
                        return Results.BadRequest("ProcedureType is required.");
                    }

                    if (string.IsNullOrWhiteSpace(request.Reason))
                    {
                        return Results.BadRequest("Reason is required.");
                    }

                    var procedureRequestId = Guid.NewGuid();
                    var historyEntry = new MedicalHistoryEntry
                    {
                        Id = Guid.NewGuid(),
                        TreatmentId = treatment.Id,
                        PatientId = treatment.PatientId,
                        AppointmentId = treatment.AppointmentId,
                        PhysicianId = treatment.PhysicianId,
                        EntryType = "ProcedureRequest",
                        Notes = $"{request.ProcedureType}: {request.Reason}",
                        Timestamp = DateTime.UtcNow,
                    };

                    db.MedicalHistoryEntries.Add(historyEntry);
                    await db.SaveChangesAsync();

                    await publishEndpoint.Publish(
                        new ProcedureRequested
                        {
                            ProcedureRequestId = procedureRequestId,
                            TreatmentId = treatment.Id,
                            AppointmentId = treatment.AppointmentId ?? Guid.Empty,
                            PatientId = treatment.PatientId,
                            PhysicianId = treatment.PhysicianId ?? Guid.Empty,
                            ProcedureType = request.ProcedureType,
                            Reason = request.Reason,
                            RequestedAt = historyEntry.Timestamp,
                        }
                    );

                    await publishEndpoint.Publish(
                        new MedicalHistoryAdded
                        {
                            HistoryEntryId = historyEntry.Id,
                            TreatmentId = treatment.Id,
                            AppointmentId = treatment.AppointmentId,
                            PatientId = treatment.PatientId,
                            PhysicianId = historyEntry.PhysicianId ?? Guid.Empty,
                            EntryType = historyEntry.EntryType,
                            AddedAt = historyEntry.Timestamp,
                        }
                    );

                    await PublishMedicalActivityAsync(
                        publishEndpoint,
                        treatment,
                        "ProcedureRequested",
                        "PROCEDURE_REQUEST",
                        historyEntry.Notes
                    );

                    return Results.Accepted(
                        $"/medical-treatments/{treatment.Id}",
                        new
                        {
                            ProcedureRequestId = procedureRequestId,
                            HistoryEntryId = historyEntry.Id,
                        }
                    );
                }
            )
            .WithName("RequestProcedure");

        return app;
    }

    private static string BuildMedicationDetails(PrescribeMedicationRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.MedicationDetails))
        {
            return request.MedicationDetails.Trim();
        }

        var details = new[] { request.MedicationName, request.Dosage, request.Instructions }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim());

        return string.Join(" - ", details);
    }

    private static Task PublishMedicalActivityAsync(
        IPublishEndpoint publishEndpoint,
        MedicalTreatment treatment,
        string activityType,
        string billingCode,
        string? description
    )
    {
        return publishEndpoint.Publish(
            new MedicalActivityRegistered
            {
                ActivityId = Guid.NewGuid(),
                PatientId = treatment.PatientId,
                AppointmentId = treatment.AppointmentId ?? Guid.Empty,
                TreatmentId = treatment.Id,
                ActivityType = activityType,
                BillingCode = billingCode,
                Description = description,
                RegisteredAt = DateTime.UtcNow,
            }
        );
    }

    public record CreatePatientExamRequest(
        Guid? AppointmentId,
        Guid PatientId,
        Guid PhysicianId,
        string? TreatmentType,
        string? Results,
        string? Diagnosis,
        string Notes
    );

    public record AddMedicalHistoryNoteRequest(Guid? PhysicianId, string? EntryType, string Notes);

    public record PrescribeMedicationRequest(
        string? MedicationDetails,
        string? MedicationName,
        string? Dosage,
        string? Instructions,
        DateTime? ValidUntil
    );

    public record RequestProcedureRequest(string ProcedureType, string Reason);
}
