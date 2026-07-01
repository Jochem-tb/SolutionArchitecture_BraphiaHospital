FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/PatientCare/PatientCare.csproj", "src/PatientCare/"]
COPY ["src/Data/Data.csproj", "src/Data/"]
COPY NuGet.config .

RUN dotnet restore "src/PatientCare/PatientCare.csproj"

COPY src/PatientCare/ src/PatientCare/
COPY src/Data/ src/Data/

RUN dotnet publish "src/PatientCare/PatientCare.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PatientCare.dll"]
