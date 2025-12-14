FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LearningPlatform.BackgroundTasks/LearningPlatform.BackgroundTasks.csproj", "LearningPlatform.BackgroundTasks/"]
COPY ["LearningPlatform.Core/LearningPlatform.Core.csproj", "LearningPlatform.Core/"]
COPY ["LearningPlatform.Data/LearningPlatform.Data.csproj", "LearningPlatform.Data/"]
COPY ["LearningPlatform.Common/LearningPlatform.Common.csproj", "LearningPlatform.Common/"]
RUN dotnet restore "LearningPlatform.BackgroundTasks/LearningPlatform.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/LearningPlatform.BackgroundTasks"
RUN dotnet build "LearningPlatform.BackgroundTasks.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LearningPlatform.BackgroundTasks.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LearningPlatform.BackgroundTasks.dll"]

