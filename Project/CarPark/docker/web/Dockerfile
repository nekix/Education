# Step 0: Base
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

RUN apt-get update && \
    apt-get install -y libicu-dev curl && \
    rm -rf /var/lib/apt/lists/*

USER $APP_UID
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

# Step 1: Build
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy solution and projects
COPY ["src/Web/CarPark.Web/CarPark.Web.csproj", "CarPark/"]

COPY ["src/Application/CarPark.Application/CarPark.Application.csproj", "CarPark.Application/"]
COPY ["src/Domain/CarPark.Domain/CarPark.Domain.csproj", "CarPark.Domain/"]
COPY ["src/Infrastracture/CarPark.Infrastructure/CarPark.Infrastructure.csproj", "CarPark.Infrastructure/"]
COPY ["src/Application/CarPark.Application.Shared/CarPark.Application.Shared.csproj", "CarPark.Application.Shared/"]
COPY ["src/Infrastracture/CarPark.Infrastructure.Geo/CarPark.Infrastructure.Geo.csproj", "CarPark.Infrastructure.Geo/"]
COPY ["src/Infrastracture/CarPark.Infrastructure.TimeZones/CarPark.Infrastructure.TimeZones.csproj", "CarPark.Infrastructure.TimeZones/"]
RUN dotnet restore "./CarPark/CarPark.Web.csproj"
COPY . .
WORKDIR "/src/CarPark"
RUN dotnet build "./CarPark.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Step 2: Publish
# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CarPark.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Step 3: Final
# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN ln -sf /app/runtimes/linux-x64/native/libicuuc.so.72.1.0.3 \
           /app/runtimes/linux-x64/native/libicuuc.so.72 && \
    ln -sf /app/runtimes/linux-x64/native/libicudata.so.72.1.0.3 \
           /app/runtimes/linux-x64/native/libicudata.so.72 && \
    ln -sf /app/runtimes/linux-x64/native/libicui18n.so.72.1.0.3 \
           /app/runtimes/linux-x64/native/libicui18n.so.72
USER $APP_UID

ENTRYPOINT ["dotnet", "CarPark.Web.dll"]

EXPOSE 8080
EXPOSE 8081