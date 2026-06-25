# Etapa 1: Build y Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["SportTrack-v1.Api/SportTrack-v1.Api.csproj", "SportTrack-v1.Api/"]
COPY ["SportTrack-v1.AccesoDatos/SportTrack-v1.AccesoDatos.csproj", "SportTrack-v1.AccesoDatos/"]
COPY ["SportTrack-v1.Controladores/SportTrack-v1.Controladores.csproj", "SportTrack-v1.Controladores/"]
COPY ["SportTrack-v1.Entidades/SportTrack-v1.Entidades.csproj", "SportTrack-v1.Entidades/"]

RUN dotnet restore "SportTrack-v1.Api/SportTrack-v1.Api.csproj"

# Copiar el resto del código y compilar
COPY . .
WORKDIR "/src/SportTrack-v1.Api"
RUN dotnet build "SportTrack-v1.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SportTrack-v1.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar postgresql-client para disponer de pg_dump (necesario para respaldos de base de datos)
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Exponer el puerto predeterminado de Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Desactivar file watchers (fix para Render free tier - límite de inotify)
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENTRYPOINT ["dotnet", "SportTrack-v1.Api.dll"]
