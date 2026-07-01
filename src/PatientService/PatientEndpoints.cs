using PatientService;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Data.Contracts;
using Data.DbContexts;
using Data.Models;

namespace PatientService;



public static class PatientEndpoints
{
    public static WebApplication MapPatientEndPoints(this WebApplication app)
    {
        var patientGroup = app.MapGroup("/api/patient");
        patientGroup.MapPost("/register", async ([FromBody] PatientRegisterRequest request, IPublishEndpoint publishEndpoint, PatientDbContext dbContext) =>
        {
            var existingPatient = dbContext.Patients.FirstOrDefault(p => p.InsuranceNumber == request.insuranceNumber);

            if (existingPatient is not null)
            {
                return Results.Conflict(
                    new { Message = "Patient already exists." });
            }

            var patient = new Patient()
            {
                PatientId = Guid.NewGuid(),
                FirstName = request.firstName,
                LastName = request.lastName,
                PhoneNumber = request.phoneNumber,
                InsuranceNumber = request.insuranceNumber
            };
            var message = new PatientRegistered
            {
                PatientId = patient.PatientId,
                Name = patient.FirstName + " " + patient.LastName,
            };
            dbContext.Patients.Add(patient);
            await dbContext.SaveChangesAsync();
            await publishEndpoint.Publish(message);

            return Results.Ok(new { Message = "Patient created", PatientId = patient.PatientId, Name = patient.FirstName + " " + patient.LastName });
        });
      
        return app;
    }

    public record PatientRegisterRequest(string firstName, string lastName, string insuranceNumber, string phoneNumber );
}