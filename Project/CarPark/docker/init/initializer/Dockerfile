# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files in dependency order
COPY ["src/Domain/CarPark.Domain/CarPark.Domain.csproj", "src/Domain/CarPark.Domain/"]
COPY ["src/Infrastracture/CarPark.Infrastructure/CarPark.Infrastructure.csproj", "src/Infrastracture/CarPark.Infrastructure/"]
COPY ["src/Infrastracture/CarPark.Infrastructure.Geo/CarPark.Infrastructure.Geo.csproj", "src/Infrastracture/CarPark.Infrastructure.Geo/"]
COPY ["src/Infrastracture/CarPark.Infrastructure.TimeZones/CarPark.Infrastructure.TimeZones.csproj", "src/Infrastracture/CarPark.Infrastructure.TimeZones/"]
COPY ["src/DataGeneration/CarPark.DataGenerator/CarPark.DataGenerator.csproj", "src/DataGeneration/CarPark.DataGenerator/"]
COPY ["src/DataGeneration/CarPark.TrackGenerator/CarPark.TrackGenerator.csproj", "src/DataGeneration/CarPark.TrackGenerator/"]
COPY ["src/DataGeneration/CarPark.Initializer/CarPark.Initializer.csproj", "src/DataGeneration/CarPark.Initializer/"]

# Restore dependencies
RUN dotnet restore "src/DataGeneration/CarPark.Initializer/CarPark.Initializer.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/DataGeneration/CarPark.Initializer"
RUN dotnet publish "CarPark.Initializer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base_init
WORKDIR /app

# Install runtime dependencies
RUN apt-get update && \
    apt-get install -y postgresql-client libicu-dev && \
    rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Create ICU symlinks for timezone support
USER root
RUN ln -sf /app/runtimes/linux-x64/native/libicuuc.so.72.1.0.3 \
           /app/runtimes/linux-x64/native/libicuuc.so.72 && \
    ln -sf /app/runtimes/linux-x64/native/libicudata.so.72.1.0.3 \
           /app/runtimes/linux-x64/native/libicudata.so.72 && \
    ln -sf /app/runtimes/linux-x64/native/libicui18n.so.72.1.0.3 \
           /app/runtimes/linux-x64/native/libicui18n.so.72
USER $APP_UID

FROM base_init AS demo

# Run init initialization
CMD dotnet CarPark.Initializer.dll init demo --connection-string "Host=postgres;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD;Timezone=UTC" --graphhopper-api-key "$GRAPHHOPPER_API_KEY"

FROM base_init AS minimal

# Run minimal initialization
CMD dotnet CarPark.Initializer.dll init minimal --connection-string "Host=postgres;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD;Timezone=UTC"