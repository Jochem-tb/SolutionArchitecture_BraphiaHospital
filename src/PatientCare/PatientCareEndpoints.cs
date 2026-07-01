namespace PatientCare;

public static class PatientCareEndpoints
{
    public static WebApplication MapPatientCareEndpoints(this WebApplication app)
    {
        app.MapGet("/patient-care/health", () => Results.Ok(new { Status = "ok" }))
            .WithName("GetPatientCareHealth");

        return app;
    }
}
