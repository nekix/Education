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
# Copy solution and projects in dependency order
COPY ["src/Domain/CarPark.Domain/CarPark.Domain.csproj", "src/Domain/CarPark.Domain/"]
COPY ["src/Infrastracture/CarPark.Infrastructure/CarPark.Infrastructure.csproj", "src/Infrastracture/CarPark.Infrastructure/"]
COPY ["src/Infrastracture/CarPark.Infrastructure.Geo/CarPark.Infrastructure.Geo.csproj", "src/Infrastracture/CarPark.Infrastructure.Geo/"]
COPY ["src/Infrastracture/CarPark.Infrastructure.TimeZones/CarPark.Infrastructure.TimeZones.csproj", "src/Infrastracture/CarPark.Infrastructure.TimeZones/"]
COPY ["src/Application/CarPark.Application/CarPark.Application.csproj", "src/Application/CarPark.Application/"]
COPY ["src/Application/CarPark.Application.Shared/CarPark.Application.Shared.csproj", "src/Application/CarPark.Application.Shared/"]
COPY ["src/Web/CarPark.Web/CarPark.Web.csproj", "src/Web/CarPark.Web/"]

RUN dotnet restore "src/Web/CarPark.Web/CarPark.Web.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "src/Web/CarPark.Web/CarPark.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

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