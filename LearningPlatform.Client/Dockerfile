FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LearningPlatform.Client/LearningPlatform.Client.csproj", "LearningPlatform.Client/"]
COPY ["LearningPlatform.Common/LearningPlatform.Common.csproj", "LearningPlatform.Common/"]
RUN dotnet restore "LearningPlatform.Client/LearningPlatform.Client.csproj"
COPY . .
WORKDIR "/src/LearningPlatform.Client"
RUN dotnet build "LearningPlatform.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LearningPlatform.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LearningPlatform.Client.dll"]

