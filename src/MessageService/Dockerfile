FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/MessageService/MessageService.csproj", "src/MessageService/"]
COPY NuGet.config .
RUN dotnet restore "src/MessageService/MessageService.csproj"

COPY src/Data/ src/Data/
COPY src/MessageService/ src/MessageService/
RUN dotnet publish "src/MessageService/MessageService.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MessageService.dll"]