FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Services/Auth/Auth.API/Auth.API.csproj Services/Auth/Auth.API/

COPY BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/BuildingBlocks/
COPY BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj BuildingBlocks/BuildingBlocks.Messaging/

RUN dotnet restore Services/Auth/Auth.API/Auth.API.csproj

COPY . .
WORKDIR /src/Services/Auth/Auth.API
RUN dotnet publish Auth.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY Services/Auth/Auth.API/keys ./keys
ENTRYPOINT ["dotnet", "Auth.API.dll"]
