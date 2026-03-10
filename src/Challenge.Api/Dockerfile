# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Challenge.Api/Challenge.Api.csproj", "Challenge.Api/"]
COPY ["Challenge.Application/Challenge.Application.csproj", "Challenge.Application/"]
COPY ["Challenge.Domain/Challenge.Domain.csproj", "Challenge.Domain/"]
COPY ["Challenge.Infra/Challenge.Infra.csproj", "Challenge.Infra/"]
COPY ["Challenge.Infra.CrossCutting/Challenge.Infra.CrossCutting.csproj", "Challenge.Infra.CrossCutting/"]
RUN dotnet restore "./Challenge.Api/Challenge.Api.csproj"
COPY . .
WORKDIR "/src/Challenge.Api"
RUN dotnet build "./Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Challenge.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Challenge.Api.dll"]