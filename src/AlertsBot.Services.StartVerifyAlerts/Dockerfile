FROM mcr.microsoft.com/dotnet/core/runtime:3.0-bionic AS base
WORKDIR / app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic AS build

WORKDIR /src
COPY ["src/AlertsBot.Services.StartVerifyAlerts/AlertsBot.Services.StartVerifyAlerts.csproj", "src/AlertsBot.Services.StartVerifyAlerts/"]
COPY ["src/AlertsBot.Domain/AlertsBot.Domain.csproj", "src/AlertsBot.Domain/"]
COPY ["src/AlertsBot.Domain.Events.Core/AlertsBot.Domain.Events.Core.csproj", "src/AlertsBot.Domain.Events.Core/"]
COPY ["src/AlertsBot.Infra.CrossCutting.Bus/AlertsBot.Infra.CrossCutting.Bus.csproj", "src/AlertsBot.Infra.CrossCutting.Bus/"]
COPY ["src/AlertsBot.Infra.CrossCutting.IoC/AlertsBot.Infra.CrossCutting.IoC.csproj", "src/AlertsBot.Infra.CrossCutting.IoC/"]
COPY ["src/AlertsBot.Infra.SqlServer/AlertsBot.Infra.SqlServer.csproj", "src/AlertsBot.Infra.SqlServer/"]
COPY ["src/AlertsBot.Services.Telegram/AlertsBot.Services.Telegram.csproj", "src/AlertsBot.Services.Telegram/"]


COPY ["src/nuget.config", "/."]

RUN dotnet restore "src/AlertsBot.Services.StartVerifyAlerts/AlertsBot.Services.StartVerifyAlerts.csproj"
COPY . .
WORKDIR "/src/src/AlertsBot.Services.StartVerifyAlerts"
RUN dotnet build "AlertsBot.Services.StartVerifyAlerts.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AlertsBot.Services.StartVerifyAlerts.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AlertsBot.Services.StartVerifyAlerts.dll"]
