FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["user_service_api.csproj", "./"]
RUN dotnet restore "user_service_api.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "user_service_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "user_service_api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "user_service_api.dll"]
