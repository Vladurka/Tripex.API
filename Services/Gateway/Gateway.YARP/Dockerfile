FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY Services/Gateway/Gateway.YARP/Gateway.YARP.csproj Services/Gateway/Gateway.YARP/

COPY BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/BuildingBlocks/

RUN dotnet restore Services/Gateway/Gateway.YARP/Gateway.YARP.csproj

COPY . .
WORKDIR /src/Services/Gateway/Gateway.YARP
RUN dotnet publish Gateway.YARP.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.YARP.dll"]
