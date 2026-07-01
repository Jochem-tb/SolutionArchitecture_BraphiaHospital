using Data.Contracts;
using Data.DbContexts;
using Data.Enums;
using Data.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementApi.Consumers;

public class PatientExaminedConsumer : IConsumer<PatientExamined>
{
    private readonly ILogger<PatientExaminedConsumer> _logger;
    private readonly AppointmentDbContext _appointmentDbContext;

    public PatientExaminedConsumer(ILogger<PatientExaminedConsumer> logger, AppointmentDbContext appointmentDbContext)
    {
        _logger = logger;
        _appointmentDbContext = appointmentDbContext;
    }

    public async Task Consume(ConsumeContext<PatientExamined> context)
    {
        _logger.LogInformation("Patient Examined: {Id} - AppointmentId: {AppointmentId}, PatientId: {PatientId}, PhysicianId: {PhyscicianId}, Diagnosis: {Diagnosis} at {Time}",
            context.Message.TreatmentId, context.Message.AppointmentId, context.Message.PatientId, context.Message.PhysicianId, context.Message.Diagnosis, context.Message.ExaminedAt);

        Appointment? appointment = await _appointmentDbContext.Appointments.FirstOrDefaultAsync(x => x.Id == context.Message.AppointmentId);

        if(appointment != null)
        {
            appointment.ScheduleStatus = AppointmentScheduleStatus.Completed;
            await _appointmentDbContext.SaveChangesAsync();
        }
    }
}