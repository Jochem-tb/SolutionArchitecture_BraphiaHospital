FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy the project files into their matching destination folders
COPY ["src/Contracts/Contracts.csproj", "src/Contracts/"]
COPY ["src/PatientService/PatientService.csproj", "src/PatientService/"]
COPY NuGet.config .

# 2. Restore using the service project
RUN dotnet restore "src/PatientService/PatientService.csproj"

# 3. Copy the actual source code folders before publishing
COPY src/Contracts/ src/Contracts/
COPY src/PatientService/ src/PatientService/

# 4. Publish directly from the /src root using the full path
RUN dotnet publish "src/PatientService/PatientService.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PatientService.dll"]