FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/EER.API/EER.API.csproj", "src/EER.API/"]
COPY ["src/EER.Application/EER.Application.csproj", "src/EER.Application/"]
COPY ["src/EER.Domain/EER.Domain.csproj", "src/EER.Domain/"]
COPY ["src/EER.Infrastructure/EER.Infrastructure.csproj", "src/EER.Infrastructure/"]
COPY ["src/Persistence/EER.Persistence.MongoDB/EER.Persistence.MongoDB.csproj", "src/Persistence/EER.Persistence.MongoDB/"]
COPY ["src/Persistence/EER.Persistence.Dapper/EER.Persistence.Dapper.csproj", "src/Persistence/EER.Persistence.Dapper/"]
COPY ["src/Persistence/EER.Persistence.Migrations/EER.Persistence.Migrations.csproj", "src/Persistence/EER.Persistence.Migrations/"]
COPY ["src/Persistence/EER.Persistence.EFCore/EER.Persistence.EFCore.csproj", "src/Persistence/EER.Persistence.EFCore/"]
RUN dotnet restore "src/EER.API/EER.API.csproj"
COPY . .
WORKDIR "/src/src/EER.API"
RUN dotnet build "EER.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EER.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EER.API.dll"]
