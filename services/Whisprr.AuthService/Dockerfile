FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["services/Whisprr.AuthService/Whisprr.AuthService.csproj", "services/Whisprr.AuthService/"]
COPY ["lib/Whisprr.Contracts/Whisprr.Contracts.csproj", "lib/Whisprr.Contracts/"]
COPY ["lib/Whisprr.MessageBroker/Whisprr.MessageBroker.csproj", "lib/Whisprr.MessageBroker/"]
RUN dotnet restore "services/Whisprr.AuthService/Whisprr.AuthService.csproj"
COPY . .
WORKDIR "/src/services/Whisprr.AuthService"
RUN dotnet build "Whisprr.AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Whisprr.AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Whisprr.AuthService.dll"]
