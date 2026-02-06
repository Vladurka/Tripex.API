# ===== Base image =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ===== Build image =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем всю структуру сервисов (чтобы были доступны зависимые проекты)
COPY . .

# Переходим в директорию Profiles.API
WORKDIR /src/Profiles/Profiles.API

# Восстанавливаем зависимости
RUN dotnet restore "Profiles.API.csproj"

# Собираем проект
RUN dotnet build "Profiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===== Publish =====
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Profiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final image =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Profiles.API.dll"]
