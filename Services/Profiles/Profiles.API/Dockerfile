FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Services/Profiles/Profiles.API/Profiles.API.csproj Services/Profiles/Profiles.API/
COPY Services/Profiles/Profiles.Application/Profiles.Application.csproj Services/Profiles/Profiles.Application/
COPY Services/Profiles/Profiles.Domain/Profiles.Domain.csproj Services/Profiles/Profiles.Domain/
COPY Services/Profiles/Profiles.Infrastructure/Profiles.Infrastructure.csproj Services/Profiles/Profiles.Infrastructure/

COPY BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/BuildingBlocks/
COPY BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj BuildingBlocks/BuildingBlocks.Messaging/

RUN dotnet restore Services/Profiles/Profiles.API/Profiles.API.csproj

COPY . .
WORKDIR /src/Services/Profiles/Profiles.API
RUN dotnet publish Profiles.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Profiles.API.dll"]
