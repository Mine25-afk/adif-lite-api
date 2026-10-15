# syntax=docker/dockerfile:1
# Imagen de ADIF Lite API: compila con el SDK y corre sobre el runtime de ASP.NET como usuario sin privilegios.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restaurar primero aprovecha la caché de capas cuando solo cambia el código.
COPY Directory.Build.props Directory.Packages.props ./
COPY src/Adif.Domain/Adif.Domain.csproj src/Adif.Domain/
COPY src/Adif.Application/Adif.Application.csproj src/Adif.Application/
COPY src/Adif.Infrastructure/Adif.Infrastructure.csproj src/Adif.Infrastructure/
COPY src/Adif.Api/Adif.Api.csproj src/Adif.Api/
RUN dotnet restore src/Adif.Api/Adif.Api.csproj

COPY src/ src/
RUN dotnet publish src/Adif.Api/Adif.Api.csproj -c Release -o /app --no-restore -p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app .

# APP_UID (1654) lo define la imagen base de Microsoft: el proceso no corre como root.
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Adif.Api.dll"]
