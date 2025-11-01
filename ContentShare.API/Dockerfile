FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ContentShare.API/ContentShare.API.csproj ContentShare.API/
COPY ContentShare.Application/ContentShare.Application.csproj ContentShare.Application/
COPY ContentShare.Domain/ContentShare.Domain.csproj ContentShare.Domain/
COPY ContentShare.Infrastructure/ContentShare.Infrastructure.csproj ContentShare.Infrastructure/

RUN dotnet restore ContentShare.API/ContentShare.API.csproj

COPY . .

RUN dotnet publish ContentShare.API/ContentShare.API.csproj -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
 && apt-get install -y --no-install-recommends curl \
 && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production


COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "ContentShare.API.dll"]
