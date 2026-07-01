using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Data.MigrationService;


public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(
            "Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();

            //Hier nieuwe db's toevoegen voor het uitvoeren van migraties
            var allContext = scope.ServiceProvider.GetRequiredService<AllContext>();
            var appointmentDbContext = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();
            var patientDbContext = scope.ServiceProvider.GetRequiredService<PatientDbContext>();

            await RunMigrationAsync(allContext, cancellationToken);
            await RunMigrationAsync(appointmentDbContext, cancellationToken);
            await RunMigrationAsync(patientDbContext, cancellationToken);
            //  await SeedDataAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(
        DbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }

    //private static async Task SeedDataAsync(
    //    AllContext dbContext, CancellationToken cancellationToken)
    //{
    //    SupportTicket firstTicket = new()
    //    {
    //        Title = "Test Ticket",
    //        Description = "Default ticket, please ignore!",
    //        Completed = true
    //    };

    //    var strategy = dbContext.Database.CreateExecutionStrategy();
    //    await strategy.ExecuteAsync(async () =>
    //    {
    //        // Seed the database
    //        await using var transaction = await dbContext.Database
    //            .BeginTransactionAsync(cancellationToken);

    //        await dbContext.Tickets.AddAsync(firstTicket, cancellationToken);
    //        await dbContext.SaveChangesAsync(cancellationToken);
    //        await transaction.CommitAsync(cancellationToken);
    //    });
    //}
}