# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TaskMasterPro/TaskMasterPro.csproj TaskMasterPro/
RUN dotnet restore TaskMasterPro/TaskMasterPro.csproj
COPY . .
RUN dotnet publish TaskMasterPro/TaskMasterPro.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskMasterPro.dll"]


