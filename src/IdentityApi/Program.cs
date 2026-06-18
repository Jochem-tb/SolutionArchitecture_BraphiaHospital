using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using OnlineToestemming.IdentityApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Configuration.AddEnvironmentVariables();

builder
    .Services.AddTransient<IAuthenticationService, AuthenticationService>()
    .Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineToestemmingDb"))
);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapAuthEndpoints();

app.Run();
