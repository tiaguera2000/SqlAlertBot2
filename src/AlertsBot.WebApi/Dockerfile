FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src

COPY ["src/nuget.config", "/."]
COPY ["src/AlertsBot.WebApi/AlertsBot.WebApi.csproj", "src/AlertsBot.WebApi/"]
COPY ["src/AlertsBot.Domain.Events.Core/AlertsBot.Domain.Events.Core.csproj", "src/AlertsBot.Domain.Events.Core/"]
COPY ["src/AlertsBot.Infra.CrossCutting.IoC/AlertsBot.Infra.CrossCutting.IoC.csproj", "src/AlertsBot.Infra.CrossCutting.IoC/"]

COPY ["src/AlertsBot.Domain.Core/AlertsBot.Domain.Core.csproj", "src/AlertsBot.Domain.Core/"]
COPY ["src/AlertsBot.Application/AlertsBot.Application.csproj", "src/AlertsBot.Application/"]
COPY ["src/AlertsBot.Domain/AlertsBot.Domain.csproj", "src/AlertsBot.Domain/"]
COPY ["src/AlertsBot.Infra.Data/AlertsBot.Infra.Data.csproj", "src/AlertsBot.Infra.Data/"]
COPY ["src/AlertsBot.Infra.SqlServer/AlertsBot.Infra.SqlServer.csproj", "src/AlertsBot.Infra.SqlServer/"]
COPY ["src/AlertsBot.Infra.CrossCutting.Bus/AlertsBot.Infra.CrossCutting.Bus.csproj", "src/AlertsBot.Infra.CrossCutting.Bus/"]


RUN dotnet restore "src/AlertsBot.WebApi/AlertsBot.WebApi.csproj"

COPY . .
WORKDIR "/src/src/AlertsBot.WebApi"
RUN dotnet build "AlertsBot.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AlertsBot.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AlertsBot.WebApi.dll"]