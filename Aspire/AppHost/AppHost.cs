var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("SqlPassword");

var sql = builder
    .AddSqlServer("sql", port: 2026, password: sqlPassword)
    .WithDataVolume("AspireDataVolume")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(proxyEnabled: false);

var db = sql.AddDatabase("OnlineToestemmingDb", databaseName: "OnlineToestemming");

var migration = builder.AddProject<Projects.Data_MigrationService>("Migrations")
    .WithReference(db)
    .WaitFor(db)
    .WithParentRelationship(db);

var jwtSigningKey = new GenerateParameterDefault { MinLength = 44 }.GetDefaultValue();

var secret = builder.AddParameter("mysecret", jwtSigningKey, secret: true);

var identity = builder.AddProject<Projects.IdentityApi>("IdentityApi")
    .WithReference(db)
    .WithReference(migration)
    .WaitFor(migration)
    .WithEnvironment("JwtSettings__SecretSigningKey", jwtSigningKey);

var pseudoniem = builder.AddProject<Projects.PseudoniemApi>("PseudoniemApi")
    .WithReference(db)
    .WithReference(migration)
    .WaitFor(migration)
    .WithEnvironment("JwtSettings__SecretSigningKey", jwtSigningKey);

var dossier = builder.AddProject<Projects.DossierApi>("DossierApi")
    .WithReference(db)
    .WithReference(identity)
    .WithReference(pseudoniem)
    .WithReference(migration)
    .WaitFor(migration)
    .WithEnvironment("JwtSettings__SecretSigningKey", jwtSigningKey)
    .WithReplicas(3);

builder.AddProject<Projects.PatientWebsite>("PatientWebsite")
    .WithExternalHttpEndpoints()
    .WithReference(db)
    .WithReference(identity)
    .WithReference(migration)
    .WaitFor(migration)
    .WaitFor(identity);


builder.Build().Run();