FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/MCP.Server/MCP.Server.csproj src/MCP.Server/
RUN dotnet restore src/MCP.Server/MCP.Server.csproj
COPY src/MCP.Server/ src/MCP.Server/
RUN dotnet publish src/MCP.Server/MCP.Server.csproj -c Release --no-restore -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 5000
ENTRYPOINT ["dotnet", "MCP.Server.dll"]
