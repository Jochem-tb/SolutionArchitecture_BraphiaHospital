using Data.DbContexts;
using Data.MigrationService;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

//Voeg database toe aan project, de database naam komt uit de connection string en die worden momenteel in docker-compose.yml gedefineerd
builder.Services.AddDbContext<AppointmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppointmentManagementDb"))
);
builder.Services.AddDbContext<PatientDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PatientManagementDb"))
);
//Dit zorgt ervoor dat de migraties worden uitgevoerd voor alle db's
builder.Services.AddHostedService<Worker>();

//Dus bij aanmaken van een nieuwe db een AddDbContext toevoegen en in de worker een lijn toevoegen de migraties uitvoeren voor de nieuwe d

//Voor het aanmaken van de eerste migraties moet je ff de output-dir specificeren
//dotnet ef migrations add InitialMigration --context DB1Context --output-dir DbMigrations/OnlineToestemmingDb

var host = builder.Build();
host.Run();