# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ["RubricasApp.Web.csproj", "./"]
RUN dotnet restore "RubricasApp.Web.csproj"

# Copiar todo el código y compilar
COPY . .
RUN dotnet publish "RubricasApp.Web.csproj" -c Release -o /app/publish

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar dependencias para System.Drawing
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Variables de entorno para producción
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_PRINT_TELEMETRY_MESSAGE=false
ENV ASPNETCORE_URLS=http://0.0.0.0:10000

EXPOSE 10000

# Habilitar logs detallados en caso de error
ENTRYPOINT ["dotnet", "RubricasApp.Web.dll"]
