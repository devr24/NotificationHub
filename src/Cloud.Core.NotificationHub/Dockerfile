# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build images for debugging.
# Based on buster slim - Debian 10 - https://hub.docker.com/_/microsoft-dotnet-core-runtime/ with asp.net 3.1 runtime.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use sdk for debugging.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

# Restore and build.
WORKDIR /src
COPY ["Cloud.Core.NotificationHub/Cloud.Core.NotificationHub.csproj", "Cloud.Core.NotificationHub/"]
RUN dotnet restore "Cloud.Core.NotificationHub/Cloud.Core.NotificationHub.csproj"
COPY . .
WORKDIR "/src/Cloud.Core.NotificationHub"
RUN dotnet build "Cloud.Core.NotificationHub.csproj" -c Release -o /app/build

# publish.
FROM build AS publish
RUN dotnet publish "Cloud.Core.NotificationHub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# entry point for app.
ENTRYPOINT ["dotnet", "Cloud.Core.NotificationHub.dll"]