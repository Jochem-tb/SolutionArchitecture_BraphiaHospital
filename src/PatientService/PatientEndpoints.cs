using PatientService;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
namespace PatientService;



public static class PatientEndpoints
{
    public static WebApplication MapPatientEndPoints(this WebApplication app)
    {
        app.MapPost("/patients", async ([FromBody] PatientRegisterRequest request, IPublishEndpoint publishEndpoint) =>
        {
            var message = new PatientRegistered
            {
                PatientId = Guid.NewGuid().ToString(),
                Name = request.Name
            };

            await publishEndpoint.Publish(message);

            return Results.Ok(new { Message = "Event Dispatched!", PatientId = message.PatientId });
        });
        return app;
    }

    public record PatientRegisterRequest(string Name);
}