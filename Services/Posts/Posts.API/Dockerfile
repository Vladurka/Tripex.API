# ===== Base image =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ===== Build image =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем ВСЁ из контекста (важно!)
COPY . .

# Переходим в проект Posts.API
WORKDIR /src/Posts/Posts.API

# Восстанавливаем зависимости и билдим
RUN dotnet restore "Posts.API.csproj"
RUN dotnet build "Posts.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===== Publish =====
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Posts.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final image =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Posts.API.dll"]
