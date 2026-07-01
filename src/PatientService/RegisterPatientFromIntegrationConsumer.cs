using System.Security.Cryptography;
using System.Text;
using Data.Contracts;
using Data.DbContexts;
using Data.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PatientService.Consumers;

public class RegisterPatientFromIntegrationConsumer : IConsumer<PatientRegisteredFromRegistration>
{
    private readonly PatientDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RegisterPatientFromIntegrationConsumer> _logger;

    public RegisterPatientFromIntegrationConsumer(PatientDbContext dbContext, IPublishEndpoint publishEndpoint, ILogger<RegisterPatientFromIntegrationConsumer> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PatientRegisteredFromRegistration> context)
    {
        var message = context.Message;

        // De PatientService bepaalt zelf hoe de Natural Key / InsuranceNumber wordt opgebouwd
        string rawKey = $"{message.FirstName.ToLower()}-{message.LastName.ToLower()}-{message.PhoneNumber.Replace(" ", "")}";
        string generatedInsuranceNumber = GenerateDeterministicId(rawKey);

        // Idempotency check binnen de eigen database grenzen
        var existingPatient = _dbContext.Patients.FirstOrDefault(p => p.InsuranceNumber == generatedInsuranceNumber);
        if (existingPatient is not null)
        {
            _logger.LogInformation("Patiënt {FirstName} {LastName} bestaat al. Integratiecommando overgeslagen.", message.FirstName, message.LastName);
            return;
        }

        var patient = new Patient
        {
            PatientId = Guid.NewGuid(),
            FirstName = message.FirstName,
            LastName = message.LastName,
            PhoneNumber = message.PhoneNumber,
            InsuranceNumber = generatedInsuranceNumber
        };

        _dbContext.Patients.Add(patient);
        await _dbContext.SaveChangesAsync();

        // Publiceer het uiteindelijke domeinevent
        await _publishEndpoint.Publish(new PatientRegistered
        {
            PatientId = patient.PatientId,
            Name = $"{patient.FirstName} {patient.LastName}"
        });

        _logger.LogInformation("Patiënt {FirstName} {LastName} succesvol geregistreerd vanuit integratie.", patient.FirstName, patient.LastName);
    }

    private static string GenerateDeterministicId(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);
        var sb = new StringBuilder("INS-");
        for (int i = 0; i < 6; i++) sb.Append(hashBytes[i].ToString("X2"));
        return sb.ToString();
    }
}