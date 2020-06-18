FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/AlertsBot.DomainEvents.Service/AlertsBot.DomainEvents.Service.csproj", "src/AlertsBot.DomainEvents.Service/"]
COPY ["src/AlertsBot.Application/AlertsBot.Application.csproj", "src/AlertsBot.Application/"]
COPY ["src/AlertsBot.Domain/AlertsBot.Domain.csproj", "src/AlertsBot.Domain/"]
COPY ["src/AlertsBot.Domain.Events.Core/AlertsBot.Domain.Events.Core.csproj", "src/AlertsBot.Domain.Events.Core/"]
COPY ["src/AlertsBot.Infra.CrossCutting.IoC/AlertsBot.Infra.CrossCutting.IoC.csproj", "src/AlertsBot.Infra.CrossCutting.IoC/"]
COPY ["src/AlertsBot.Services.Telegram/AlertsBot.Services.Telegram.csproj", "src/AlertsBot.Services.Telegram/"]

COPY ["src/nuget.config", "/."]
RUN dotnet restore "src/AlertsBot.DomainEvents.Service/AlertsBot.DomainEvents.Service.csproj"
COPY . .
WORKDIR "/src/src/AlertsBot.DomainEvents.Service"

RUN rm /src/global.json

RUN dotnet build "AlertsBot.DomainEvents.Service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AlertsBot.DomainEvents.Service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AlertsBot.DomainEvents.Service.dll"]