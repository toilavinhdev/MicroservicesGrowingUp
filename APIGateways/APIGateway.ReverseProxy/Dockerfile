FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["APIGateways/APIGateway.ReverseProxy/APIGateway.ReverseProxy.csproj", "APIGateways/APIGateway.ReverseProxy/"]
RUN dotnet restore "APIGateways/APIGateway.ReverseProxy/APIGateway.ReverseProxy.csproj"
COPY . .
WORKDIR "/src/APIGateways/APIGateway.ReverseProxy"
RUN dotnet build "APIGateway.ReverseProxy.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "APIGateway.ReverseProxy.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGateway.ReverseProxy.dll"]
