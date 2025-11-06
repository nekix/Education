# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["CarPark.DbMigrator/CarPark.DbMigrator.csproj", "CarPark.DbMigrator/"]
COPY ["CarPark.Shared/CarPark.Shared.csproj", "CarPark.Shared/"]
COPY ["CarPark.Domain/CarPark.Domain.csproj", "CarPark.Domain/"]
COPY ["CarPark.Infrastructure/CarPark.Infrastructure.csproj", "CarPark.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "CarPark.DbMigrator/CarPark.DbMigrator.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/CarPark.DbMigrator"
RUN dotnet publish "CarPark.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

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