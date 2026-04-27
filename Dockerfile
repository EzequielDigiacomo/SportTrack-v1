# Estructura Multi-Proyecto para Render
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto (.csproj) primero para cachear capas
COPY ["SportTrack-v1.Api/SportTrack-v1.Api.csproj", "SportTrack-v1.Api/"]
COPY ["SportTrack-v1.Entidades/SportTrack-v1.Entidades.csproj", "SportTrack-v1.Entidades/"]
COPY ["SportTrack-v1.AccesoDatos/SportTrack-v1.AccesoDatos.csproj", "SportTrack-v1.AccesoDatos/"]
COPY ["SportTrack-v1.Controladores/SportTrack-v1.Controladores.csproj", "SportTrack-v1.Controladores/"]

# Restaurar dependencias
RUN dotnet restore "SportTrack-v1.Api/SportTrack-v1.Api.csproj"

# Copiar el resto del código
COPY . .

# Publicar la aplicación
WORKDIR "/src/SportTrack-v1.Api"
RUN dotnet publish "SportTrack-v1.Api.csproj" -c Release -o /app/publish

# Imagen final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render usa el puerto 8080 por defecto para Docker
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SportTrack-v1.Api.dll"]
