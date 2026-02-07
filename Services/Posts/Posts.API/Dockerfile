FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Services/Posts/Posts.API/Posts.API.csproj Services/Posts/Posts.API/
COPY Services/Posts/Posts.Application/Posts.Application.csproj Services/Posts/Posts.Application/
COPY Services/Posts/Posts.Domain/Posts.Domain.csproj Services/Posts/Posts.Domain/
COPY Services/Posts/Posts.Infrastructure/Posts.Infrastructure.csproj Services/Posts/Posts.Infrastructure/

COPY BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/BuildingBlocks/
COPY BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj BuildingBlocks/BuildingBlocks.Messaging/

RUN dotnet restore Services/Posts/Posts.API/Posts.API.csproj

COPY . .
WORKDIR /src/Services/Posts/Posts.API
RUN dotnet publish Posts.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Posts.API.dll"]
