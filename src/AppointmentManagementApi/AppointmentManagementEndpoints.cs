using Data.Contracts;
using Data.DbContexts;
using Data.Enums;
using Data.Models;
using EventStore.Client;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AppointmentManagementApi;

public static class AppointmentManagementEndpoints
{
    public static WebApplication MapAppointmentManagementEndpoints(this WebApplication app)
    {
        app.MapPost("/appointment", async ([FromBody] ScheduleAppointmentRequest request, AppointmentDbContext db, IPublishEndpoint publishEndpoint, EventStoreClient client) =>
        {

            PatientSmall? patient = await db.Patients.FirstOrDefaultAsync(x => x.Id == request.PatientId);

            if(patient == null)
            {
                return Results.NotFound( new { ErrorMessage = "Unknown patient id" });
            }

            if (request.DateTime <= DateTime.Today)
            {
                return Results.BadRequest(new { ErrorMessage = "DateTime cannot be today or in the past" });
            }

            Appointment newAppointment = new Appointment()
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                DateTime = request.DateTime,
                PhysicianId = Guid.NewGuid(),
                ReferalId = Guid.NewGuid(),
                ArrivalStatus = AppointmentArrivalStatus.Pending,
                ScheduleStatus = AppointmentScheduleStatus.Scheduled
            };

            await db.Appointments.AddAsync(newAppointment);
            await db.SaveChangesAsync();

            var appointmentPlanned = new AppointmentPlanned
            {
                PatientId = newAppointment.PatientId,
                AppointmentId = newAppointment.Id,
                PhysicianId = newAppointment.PhysicianId,
                ReferralId = newAppointment.ReferalId,
                ScheduledAt = newAppointment.DateTime
            };

            await publishEndpoint.Publish(appointmentPlanned);

            var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(appointmentPlanned);
            var eventData = new EventData(Uuid.NewUuid(), nameof(appointmentPlanned), utf8Bytes.AsMemory());
            await client.AppendToStreamAsync(AppointmentPlanned.StreamName, StreamState.Any, [eventData]);

            PatientDto patientDto = new(patient.Id, patient.Name);
            AppointmentDto appointmentDto = new(
                newAppointment.Id, 
                patientDto, 
                newAppointment.DateTime, 
                newAppointment.PhysicianId, 
                newAppointment.ReferalId, 
                newAppointment.ArrivalStatus.ToString(), 
                newAppointment.ScheduleStatus.ToString()
            );

            return Results.Ok(new { Message = "New appointment scheduled", newAppointment = appointmentDto });
        });

        app.MapPost("/appointment/reschedule", async ([FromBody] RescheduleAppointmentRequest request, AppointmentDbContext db, IPublishEndpoint publishEndpoint, EventStoreClient client) =>
        {
            Appointment? appointment = await db.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(x => x.Id == request.AppointmentId);

            if (appointment == null)
            {
                return Results.NotFound(new { ErrorMessage = "Unknown Appointment id" });
            }

            if(appointment.DateTime < DateTime.Now)
            {
                return Results.BadRequest(new { ErrorMessage = "Cannot reschedule a past appointment" });
            }

            if (request.DateTime <= DateTime.Today)
            {
                return Results.BadRequest(new { ErrorMessage = "DateTime cannot be today or in the past" });
            }

            var appointmentRescheduled = new AppointmentRescheduled
            {
                PatientId = appointment.PatientId,
                AppointmentId = appointment.Id,
                PhysicianId = appointment.PhysicianId,
                OldScheduledAt = appointment.DateTime,
                NewScheduledAt = request.DateTime
            };

            appointment.DateTime = request.DateTime;
            appointment.ScheduleStatus = AppointmentScheduleStatus.Rescheduled;

            await db.SaveChangesAsync();

            await publishEndpoint.Publish(appointmentRescheduled);

            var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(appointmentRescheduled);
            var eventData = new EventData(Uuid.NewUuid(), nameof(appointmentRescheduled), utf8Bytes.AsMemory());
            await client.AppendToStreamAsync(AppointmentRescheduled.StreamName, StreamState.Any, [eventData]);

            PatientDto patientDto = new(appointment.Patient.Id, appointment.Patient.Name);
            AppointmentDto appointmentDto = new(
                appointment.Id,
                patientDto,
                appointment.DateTime,
                appointment.PhysicianId,
                appointment.ReferalId,
                appointment.ArrivalStatus.ToString(),
                appointment.ScheduleStatus.ToString()
            );

            return Results.Ok(new { Message = "Appointment DateTime updated", updatedAppointment = appointmentDto });
        });

        app.MapPost("/appointment/check-in", async ([FromBody] AppointmentCheckInRequest request, AppointmentDbContext db, IPublishEndpoint publishEndpoint, EventStoreClient client) =>
        {
            Appointment? appointment = await db.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(x => x.Id == request.AppointmentId);

            if (appointment == null)
            {
                return Results.NotFound(new { ErrorMessage = "Unknown Appointment id" });
            }

            appointment.ArrivalStatus = request.ArrivalStatus;

            await db.SaveChangesAsync();

            var patientCheckedIn = new PatientCheckedIn
            {
                PatientId = appointment.PatientId,
                AppointmentId = appointment.Id,
                PhysicianId = appointment.PhysicianId,
                ArrivalStatus = request.ArrivalStatus
            };

            await publishEndpoint.Publish(patientCheckedIn);

            var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(patientCheckedIn);
            var eventData = new EventData(Uuid.NewUuid(), nameof(patientCheckedIn), utf8Bytes.AsMemory());
            await client.AppendToStreamAsync(PatientCheckedIn.StreamName, StreamState.Any, [eventData]);

            PatientDto patientDto = new(appointment.Patient.Id, appointment.Patient.Name);
            AppointmentDto appointmentDto = new(
                appointment.Id,
                patientDto,
                appointment.DateTime,
                appointment.PhysicianId,
                appointment.ReferalId,
                appointment.ArrivalStatus.ToString(),
                appointment.ScheduleStatus.ToString()
            );

            return Results.Ok(new { Message = "Appointment arrival status updated", updatedAppointment = appointmentDto });
        });

        app.MapPost("/appointment/survey", async ([FromBody] AppointmentSurveyRequest request, AppointmentDbContext db, IPublishEndpoint publishEndpoint, EventStoreClient client) =>
        {
            Appointment? appointment = await db.Appointments.Include(a => a.Patient).Include(a => a.HealthQuestionaire).FirstOrDefaultAsync(x => x.Id == request.AppointmentId);

            if (appointment == null)
            {
                return Results.NotFound(new { ErrorMessage = "Unknown Appointment id" });
            }

            appointment.AddOrUpdateQuestionnaire(request.SurveyJson);

            await db.SaveChangesAsync();

            var surveyCompleted = new SurveyCompleted
            {
                PatientId = appointment.PatientId,
                AppointmentId = appointment.Id,
                HealthDataJSON = appointment.HealthQuestionaire!.HealthDataJSON,
                SurveyId = appointment.HealthQuestionaire.Id
            };

            await publishEndpoint.Publish(surveyCompleted);

            var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(surveyCompleted);
            var eventData = new EventData(Uuid.NewUuid(), nameof(surveyCompleted), utf8Bytes.AsMemory());
            await client.AppendToStreamAsync(SurveyCompleted.StreamName, StreamState.Any, [eventData]);

            HealthQuestionaireDto healthQuestionaireDto = new(appointment.HealthQuestionaire!.Id, appointment.Id, appointment.HealthQuestionaire.HealthDataJSON);
            PatientDto patientDto = new(appointment.Patient.Id, appointment.Patient.Name);
            AppointmentDto appointmentDto = new(
                appointment.Id,
                patientDto,
                appointment.DateTime,
                appointment.PhysicianId,
                appointment.ReferalId,
                appointment.ArrivalStatus.ToString(),
                appointment.ScheduleStatus.ToString(),
                healthQuestionaireDto
            );

            return Results.Ok(new { Message = "Appointment arrival status updated", updatedAppointment = appointmentDto });
        });

        return app;
    }
    public record ScheduleAppointmentRequest(Guid PatientId, DateTime DateTime);
    public record RescheduleAppointmentRequest(Guid AppointmentId, DateTime DateTime);
    public record AppointmentCheckInRequest(Guid AppointmentId, AppointmentArrivalStatus ArrivalStatus);
    public record AppointmentSurveyRequest(Guid AppointmentId, string SurveyJson);
    public record PatientDto(Guid Id, string Name);
    public record AppointmentDto(Guid id, PatientDto patient, DateTime DateTime, Guid PhysicianId, Guid ReferalId, string ArrivalStatus, string ScheduleStatus, HealthQuestionaireDto? HealthQuestionaire = null);
    public record HealthQuestionaireDto(Guid id, Guid AppointmentId, string HealthDataJson);
}