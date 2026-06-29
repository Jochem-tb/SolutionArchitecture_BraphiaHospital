using Data.DbContexts;
using Data.Enums;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementApi;

public static class AppointmentManagementEndpoints
{
    public static WebApplication MapAppointmentManagementEndpoints(this WebApplication app)
    {
        app.MapPost("/appointment", async ([FromBody] ScheduleAppointmentRequest request, AppointmentDbContext db) =>
        {

            PatientSmall? patient = await db.Patients.FirstOrDefaultAsync(x => x.Id == request.PatientId);

            if(patient == null)
            {
                return Results.NotFound( new { ErrorMessage = "Unknown patient id" });
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

        return app;
    }
    public record ScheduleAppointmentRequest(Guid PatientId, DateTime DateTime);
    public record PatientDto(Guid Id, string Name);
    public record AppointmentDto(Guid id, PatientDto patient, DateTime DateTime, Guid PhysicianId, Guid ReferalId, string ArrivalStatus, string ScheduleStatus);
}
