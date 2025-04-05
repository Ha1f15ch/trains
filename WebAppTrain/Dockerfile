# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем все проекты
COPY ["WebAppTrain/WebAppTrain.csproj", "WebAppTrain/"]
COPY ["ApplicationDatabaseContext/DatabaseEngine/DatabaseEngine.csproj", "ApplicationDatabaseContext/DatabaseEngine/"]
COPY ["Domain/BusinesEngine/BusinesEngine.csproj", "Domain/BusinesEngine/"]
COPY ["Shared/Common/Common.csproj", "Shared/Common/"]
COPY ["Shared/DTOModels/DTOModels.csproj", "Shared/DTOModels/"]
COPY ["Shared/Integrations/Integrations.csproj", "Shared/Integrations/"]

# Восстанавливаем зависимости
RUN dotnet restore "WebAppTrain/WebAppTrain.csproj"

# Копируем остальные файлы
COPY . .

# Собираем проект
RUN dotnet build "WebAppTrain/WebAppTrain.csproj" -c Release -o /app/build

# Этап публикации
FROM build AS publish
RUN dotnet publish "WebAppTrain/WebAppTrain.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAppTrain.dll"]