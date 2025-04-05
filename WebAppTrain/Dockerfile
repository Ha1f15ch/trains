# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["WebAppTrain.csproj", "./"]
RUN dotnet restore "WebAppTrain.csproj"

# Копируем остальные файлы проекта
COPY . .
RUN dotnet build "WebAppTrain.csproj" -c Release -o /app/build

# Этап публикации
FROM build AS publish
RUN dotnet publish "WebAppTrain.csproj" -c Release -o /app/publish

# Этап финального образа
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAppTrain.dll"]