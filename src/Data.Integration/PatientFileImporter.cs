using System.IO;
using System.Text.RegularExpressions;
using Data.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Data.Integration;

public class PatientFileImporter
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ILogger<PatientFileImporter> _logger;

    public PatientFileImporter(ISendEndpointProvider sendEndpointProvider, ILogger<PatientFileImporter> logger)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _logger = logger;
    }

    public async Task ImportFromStreamAsync(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        await reader.ReadLineAsync(); // Skip header

        // We sturen het naar een queue die specifiek door de PatientService wordt uitgelezen
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:patient-registration-requests"));
        int counter = 0;

        while (await reader.ReadLineAsync() is { } line)
        {
            var tokens = ParseCsvLine(line);
            if (tokens.Length < 5) continue;

            // De integratieservice stuurt puur data door, geen database logica!
            await endpoint.Send(new PatientRegisteredFromRegistration()
            {
                CompanyName = tokens[0].Trim(),
                FirstName = tokens[1].Trim(),
                LastName = tokens[2].Trim(),
                PhoneNumber = tokens[3].Trim()
            });

            counter++;
        }

        _logger.LogInformation("{Count} integratierecords naar de PatientService queue gestuurd.", counter);
    }

    private static string[] ParseCsvLine(string line) => 
        Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
}