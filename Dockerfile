FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Homeverse.API/Homeverse.API.csproj", "Homeverse.API/"]
COPY ["Homeverse.Application/Homeverse.Application.csproj", "Homeverse.Application/"]
COPY ["Homeverse.Domain/Homeverse.Domain.csproj", "Homeverse.Domain/"]
COPY ["Homeverse.Infrastructure/Homeverse.Infrastructure.csproj", "Homeverse.Infrastructure/"]
RUN dotnet restore "./Homeverse.API/Homeverse.API.csproj"
COPY . .
WORKDIR "/src/Homeverse.API"
RUN dotnet build "./Homeverse.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Homeverse.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Homeverse.API.dll"]