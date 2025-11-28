# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files in dependency order
COPY ["src/Domain/CarPark.Domain/CarPark.Domain.csproj", "src/Domain/CarPark.Domain/"]
COPY ["src/Infrastracture/CarPark.Infrastructure/CarPark.Infrastructure.csproj", "src/Infrastracture/CarPark.Infrastructure/"]
COPY ["src/DataGeneration/CarPark.DbMigrator/CarPark.DbMigrator.csproj", "src/DataGeneration/CarPark.DbMigrator/"]

# Restore dependencies
RUN dotnet restore "src/DataGeneration/CarPark.DbMigrator/CarPark.DbMigrator.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src"
RUN dotnet publish "src/DataGeneration/CarPark.DbMigrator/CarPark.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app

# Install runtime dependencies
RUN apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Set connection string from environment
ENV ConnectionStrings__Default="Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};Timezone=UTC"

ENTRYPOINT ["dotnet", "CarPark.DbMigrator.dll"]