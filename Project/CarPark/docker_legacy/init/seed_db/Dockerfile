# Step 0: Base
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base

RUN apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

USER $APP_UID

# USER $APP_UID
WORKDIR /app

# Step 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build  
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["CarPark.Shared/CarPark.Shared.csproj", "CarPark.Shared/"]
COPY ["CarPark.Domain/CarPark.Domain.csproj", "CarPark.Domain/"]
COPY ["CarPark.Infrastructure/CarPark.Infrastructure.csproj", "CarPark.Infrastructure/"]
COPY ["CarPark.Application/CarPark.Application.csproj", "CarPark.Application/"]
COPY ["CarPark.DataGenerator/CarPark.DataGenerator.csproj", "CarPark.DataGenerator/"]
COPY ["CarPark.TrackGenerator/CarPark.TrackGenerator.csproj", "CarPark.TrackGenerator/"]

RUN dotnet restore "CarPark.DataGenerator/CarPark.DataGenerator.csproj" && \
    dotnet restore "CarPark.TrackGenerator/CarPark.TrackGenerator.csproj"

COPY . .

# Publish generators
WORKDIR "/src/CarPark.DataGenerator"
RUN dotnet publish "CarPark.DataGenerator.csproj" -c $BUILD_CONFIGURATION -o /app/publish/datagenerator --no-self-contained

WORKDIR "/src/CarPark.TrackGenerator"
RUN dotnet publish "CarPark.TrackGenerator.csproj" -c $BUILD_CONFIGURATION -o /app/publish/trackgenerator --no-self-contained

# Step 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Copy source files for EF migrations (Design package requires sources)
COPY --from=build /src/CarPark.Infrastructure ./CarPark.Infrastructure
COPY --from=build /src/CarPark.Domain ./CarPark.Domain
COPY --from=build /src/CarPark.Shared ./CarPark.Shared
COPY --from=build /src/CarPark.Application ./CarPark.Application

# Copy published generators
COPY --from=build /app/publish/datagenerator ./CarPark.DataGenerator
COPY --from=build /app/publish/trackgenerator ./CarPark.TrackGenerator

# Copy scripts and set permissions
COPY docker/init/scripts/generate-all.sh /scripts/generate-all.sh
COPY docker/init/scripts/01-run-migrations.sh /scripts/01-run-migrations.sh
COPY docker/init/scripts/02-generate-base-data.sh /scripts/02-generate-base-data.sh
COPY docker/init/scripts/03-generate-tracks.sh /scripts/03-generate-tracks.sh
COPY docker/init/scripts/04-verify-data.sh /scripts/04-verify-data.sh
# RUN mkdir -p /scripts && \
#     chmod +x /scripts/02-generate-base-data.sh && \
#     chmod +x /scripts/03-generate-tracks.sh && \
#     chmod +x /scripts/04-verify-data.sh && \
#     mkdir -p /status /tmp && \
#     chmod 777 /tmp /status

# Step 3: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /scripts /scripts

COPY --from=publish /app/publish/datagenerator /app/CarPark.DataGenerator
COPY --from=publish /app/publish/trackgenerator /app/CarPark.TrackGenerator

USER root
RUN ln -sf /app/CarPark.DataGenerator/runtimes/linux-x64/native/libicuuc.so.72.1.0.3 \
           /app/CarPark.DataGenerator/runtimes/linux-x64/native/libicuuc.so.72 && \
    ln -sf /app/CarPark.DataGenerator/runtimes/linux-x64/native/libicudata.so.72.1.0.3 \
           /app/CarPark.DataGenerator/runtimes/linux-x64/native/libicudata.so.72 && \
    ln -sf /app/CarPark.DataGenerator/runtimes/linux-x64/native/libicui18n.so.72.1.0.3 \
           /app/CarPark.DataGenerator/runtimes/linux-x64/native/libicui18n.so.72 && \
    ln -sf /app/CarPark.TrackGenerator/runtimes/linux-x64/native/libicuuc.so.72.1.0.3 \
           /app/CarPark.TrackGenerator/runtimes/linux-x64/native/libicuuc.so.72 && \
    ln -sf /app/CarPark.TrackGenerator/runtimes/linux-x64/native/libicudata.so.72.1.0.3 \
           /app/CarPark.TrackGenerator/runtimes/linux-x64/native/libicudata.so.72 && \
    ln -sf /app/CarPark.TrackGenerator/runtimes/linux-x64/native/libicui18n.so.72.1.0.3 \
           /app/CarPark.TrackGenerator/runtimes/linux-x64/native/libicui18n.so.72
USER $APP_UID

# CMD ["sleep", "infinity"]
CMD ["/scripts/generate-all.sh"]