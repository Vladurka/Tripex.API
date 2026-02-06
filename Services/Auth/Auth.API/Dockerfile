# ===== Base image =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ===== Build image =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем ВСЕ исходники (а не только один проект)
COPY . .

# Переходим в папку API
WORKDIR /src/Auth/Auth.API

# Восстанавливаем зависимости
RUN dotnet restore "Auth.API.csproj"

# Собираем проект
RUN dotnet build "Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===== Publish =====
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final image =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.API.dll"]
