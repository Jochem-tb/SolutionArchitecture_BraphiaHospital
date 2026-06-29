FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/MedicalTreatmentApi/MedicalTreatmentApi.csproj", "src/MedicalTreatmentApi/"]
COPY NuGet.config .
RUN dotnet restore "src/MedicalTreatmentApi/MedicalTreatmentApi.csproj"

COPY src/Data/ src/Data/
COPY src/MedicalTreatmentApi/ src/MedicalTreatmentApi/
RUN dotnet publish "src/MedicalTreatmentApi/MedicalTreatmentApi.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedicalTreatmentApi.dll"]
