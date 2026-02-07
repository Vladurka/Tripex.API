FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Services/Interaction/Interaction.API/Interaction.API.csproj Services/Interaction/Interaction.API/
COPY Services/Interaction/Interaction.Application/Interaction.Application.csproj Services/Interaction/Interaction.Application/
COPY Services/Interaction/Interaction.Domain/Interaction.Domain.csproj Services/Interaction/Interaction.Domain/
COPY Services/Interaction/Interaction.Infrastructure/Interaction.Infrastructure.csproj Services/Interaction/Interaction.Infrastructure/

COPY BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/BuildingBlocks/
COPY BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj BuildingBlocks/BuildingBlocks.Messaging/

RUN dotnet restore Services/Interaction/Interaction.API/Interaction.API.csproj

COPY . .
WORKDIR /src/Services/Interaction/Interaction.API
RUN dotnet publish Interaction.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Interaction.API.dll"]
