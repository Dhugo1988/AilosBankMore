FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["APIContaCorrente.csproj", "./"]
RUN dotnet restore "APIContaCorrente.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "APIContaCorrente.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIContaCorrente.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*
USER app
ENTRYPOINT ["dotnet", "APIContaCorrente.dll"]
