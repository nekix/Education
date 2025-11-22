# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["CarPark.Initializer/CarPark.Initializer.csproj", "CarPark.Initializer/"]
COPY ["CarPark.DataGenerator/CarPark.DataGenerator.csproj", "CarPark.DataGenerator/"]
COPY ["CarPark.TrackGenerator/CarPark.TrackGenerator.csproj", "CarPark.TrackGenerator/"]
COPY ["CarPark.Shared/CarPark.Shared.csproj", "CarPark.Shared/"]
COPY ["CarPark.Domain/CarPark.Domain.csproj", "CarPark.Domain/"]
COPY ["CarPark.Infrastructure/CarPark.Infrastructure.csproj", "CarPark.Infrastructure/"]
COPY ["CarPark.Application/CarPark.Application.csproj", "CarPark.Application/"]

# Restore dependencies
RUN dotnet restore "CarPark.Initializer/CarPark.Initializer.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/CarPark.Initializer"
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

# Run demo initialization
CMD dotnet CarPark.Initializer.dll init minimal --connection-string "Host=postgres;Database=$POSTGRES_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD;Timezone=UTC" --graphhopper-api-key "$GRAPHHOPPER_API_KEY"