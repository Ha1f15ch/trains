# Этап базового образа
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проектов
COPY ["WebAppTrain/WebAppTrain.csproj", "WebAppTrain/"]
COPY ["Domain/BusinesEngine/BusinesEngine.csproj", "Domain/BusinesEngine/"]
COPY ["ApplicationDatabaseContext/DatabaseEngine/DatabaseEngine.csproj", "ApplicationDatabaseContext/DatabaseEngine/"]
COPY ["Shared/Common/Common.csproj", "Shared/Common/"]
COPY ["Shared/DTOModels/DTOModels.csproj", "Shared/DTOModels/"]
COPY ["Shared/Integrations/Integrations.csproj", "Shared/Integrations/"]

# Восстанавливаем зависимости
RUN dotnet restore "./WebAppTrain/WebAppTrain.csproj"

# Копируем остальные файлы
COPY . .

# Собираем проект
WORKDIR "/src/WebAppTrain"
RUN dotnet build "./WebAppTrain.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebAppTrain.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAppTrain.dll"]