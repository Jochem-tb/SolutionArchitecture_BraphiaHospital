using Data.DbContexts;
using Data.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

//Voeg database toe aan project, de database naam komt uit de connection string en die worden momenteel in docker-compose.yml gedefineerd
builder.Services.AddDbContext<AllContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineToestemmingDb"))
    );

//Dit voert de migrations uit voor de AllContext database, hierdoor wordt de database aangemaakt als hij nog niet bestond
builder.Services.AddHostedService<Worker>();

//Dus bij aanmaken van een nieuwe db de 2 dingen hierboven aanmaken, zou ook een iets duidelijkere naam kiezen dan "Worker" zoals RunOnlineToestemmingDbMigrations

//Voor het aanmaken van de eerste migraties moet je ff de output-dir specificeren
//dotnet ef migrations add InitialMigration --context DB1Context --output-dir DbMigrations/OnlineToestemmingDb

var host = builder.Build();
host.Run();