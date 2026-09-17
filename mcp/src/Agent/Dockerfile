FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Agent/Agent.csproj src/Agent/
RUN dotnet restore src/Agent/Agent.csproj
COPY src/Agent/ src/Agent/
RUN dotnet publish src/Agent/Agent.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 5000
ENTRYPOINT ["dotnet", "Agent.dll"]
