using Data.Integration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Data.Integration;

public static class IntegrationEndpoints
{
    public static WebApplication MapIntegrationEndpoints(this WebApplication app)
    {
        app.MapGet("/integration/import-nightly", async (PatientFileImporter importer) =>
        {
            string localFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "Imports", "fake_customer_data_export.csv");

            if (!File.Exists(localFilePath))
            {
                return Results.NotFound($"Exportbestand niet gevonden op pad: {localFilePath}");
            }

            using var stream = File.OpenRead(localFilePath);
            await importer.ImportFromStreamAsync(stream);

            return Results.Ok(new { Message = "Nachtelijke batch-integratie succesvol gestart op de Message Broker." });
        });
        return app;
    }

}