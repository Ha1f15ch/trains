# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["ValidationService.csproj", "./"]
RUN dotnet restore "ValidationService.csproj"

# Копируем остальные файлы проекта
COPY . .
RUN dotnet build "ValidationService.csproj" -c Release -o /app/build

# Этап публикации
FROM build AS publish
RUN dotnet publish "ValidationService.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Копируем appsettings.json в образ
COPY appsettings.json .

ENTRYPOINT ["dotnet", "ValidationService.dll"]