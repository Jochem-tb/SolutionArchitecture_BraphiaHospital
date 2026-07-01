namespace PatientCare;

public static class PatientCareEndpoints
{
    public static WebApplication MapPatientCareEndpoints(this WebApplication app)
    {
        app.MapGet("/patient-care/health", () => Results.Ok(new { Status = "ok" }))
            .WithName("GetPatientCareHealth");

        app.MapGet(
                "/patient-care/dossiers/{patientId:guid}",
                async (
                    Guid patientId,
                    Queries.GetMedicalDossier.GetMedicalDossierQueryHandler queryHandler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await queryHandler.HandleAsync(
                        new Queries.GetMedicalDossier.GetMedicalDossierQuery(patientId),
                        cancellationToken);

                    return result is null ? Results.NotFound() : Results.Ok(result);
                })
            .WithName("GetPatientMedicalDossier");

        return app;
    }
}
