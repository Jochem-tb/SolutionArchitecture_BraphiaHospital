using MassTransit;
using PatientCare.Contracts;
using PatientCare.Infrastructure.ReadDb.Projections;

namespace PatientCare.Consumers.ReadProjections;

public sealed class ProjectionConsumer :
    IConsumer<MedicalEntryWritten>,
    IConsumer<PrescriptionWritten>,
    IConsumer<PatientCacheUpserted>,
    IConsumer<AppointmentCacheUpserted>
{
    private readonly IPatientCareReadProjector _projector;

    public ProjectionConsumer(IPatientCareReadProjector projector)
    {
        _projector = projector;
    }

    public Task Consume(ConsumeContext<MedicalEntryWritten> context)
        => _projector.Project_MedicalEntryWritten_Async(context.Message, context.CancellationToken);

    public Task Consume(ConsumeContext<PrescriptionWritten> context)
        => _projector.Project_PrescriptionWritten_Async(context.Message, context.CancellationToken);

    public Task Consume(ConsumeContext<PatientCacheUpserted> context)
        => _projector.Project_PatientCacheUpserted_Async(context.Message, context.CancellationToken);

    public Task Consume(ConsumeContext<AppointmentCacheUpserted> context)
        => _projector.Project_AppointmentCacheUpserted_Async(context.Message, context.CancellationToken);
}
