# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files first for caching
COPY ShelfService.sln ./
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Api/Api.csproj src/Api/
COPY tests/Domain.Tests/Domain.Tests.csproj tests/Domain.Tests/
COPY tests/Application.Tests/Application.Tests.csproj tests/Application.Tests/
COPY tests/Integration.Tests/Integration.Tests.csproj tests/Integration.Tests/

RUN dotnet restore

# Copy full source
COPY . .

# Build and publish
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Api.dll"]
