# Step 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only project files first (for better layer caching)
COPY ["CarPark.Shared/CarPark.Shared.csproj", "CarPark.Shared/"]
COPY ["CarPark.Domain/CarPark.Domain.csproj", "CarPark.Domain/"]
COPY ["CarPark.Infrastructure/CarPark.Infrastructure.csproj", "CarPark.Infrastructure/"]

# Restore all projects in one layer (faster, less layers)
RUN dotnet restore "CarPark.Shared/CarPark.Shared.csproj" && \
    dotnet restore "CarPark.Domain/CarPark.Domain.csproj" && \
    dotnet restore "CarPark.Infrastructure/CarPark.Infrastructure.csproj"

# Copy all source code
COPY . .

# Step 2: Final stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

# Install dotnet-ef and runtime dependencies in one layer
RUN dotnet tool install --tool-path /usr/local/bin dotnet-ef && \
    apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

# Copy source files for EF migrations (Design package requires sources)
COPY --from=build /src/CarPark.Infrastructure ./CarPark.Infrastructure
COPY --from=build /src/CarPark.Domain ./CarPark.Domain
COPY --from=build /src/CarPark.Shared ./CarPark.Shared

# Copy scripts and set permissions in one layer
COPY docker/init/scripts/01-run-migrations.sh /scripts/01-run-migrations.sh
RUN chmod +x /scripts/01-run-migrations.sh && \
    mkdir -p /app/CarPark.Infrastructure/obj && \
    chmod -R 777 /app/CarPark.Infrastructure/obj

# USER $APP_UID

CMD ["/scripts/01-run-migrations.sh"]