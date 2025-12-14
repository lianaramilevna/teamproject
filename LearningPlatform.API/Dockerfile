FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files first for better layer caching
COPY ["LearningPlatform.API/LearningPlatform.API.csproj", "LearningPlatform.API/"]
COPY ["LearningPlatform.Core/LearningPlatform.Core.csproj", "LearningPlatform.Core/"]
COPY ["LearningPlatform.Data/LearningPlatform.Data.csproj", "LearningPlatform.Data/"]
COPY ["LearningPlatform.Common/LearningPlatform.Common.csproj", "LearningPlatform.Common/"]

# Restore dependencies
RUN dotnet restore "LearningPlatform.API/LearningPlatform.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/LearningPlatform.API"
RUN dotnet build "LearningPlatform.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LearningPlatform.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LearningPlatform.API.dll"]


