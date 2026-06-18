using MassTransit;
using Contracts;
using Microsoft.Extensions.Logging;

namespace MessageService;

public class PatientRegisteredConsumer : IConsumer<PatientRegistered>
{
    private readonly ILogger<PatientRegisteredConsumer> _logger;

    public PatientRegisteredConsumer(ILogger<PatientRegisteredConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<PatientRegistered> context)
    {
        _logger.LogInformation(">>> [SUCCESS] Received Patient: {Id} - Name: {Name} at {Time}", 
            context.Message.PatientId, context.Message.Name, context.Message.RegisteredAt);
        
        return Task.CompletedTask;
    }
}