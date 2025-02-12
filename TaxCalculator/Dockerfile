# Use official .NET SDK image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaxCalculator/TaxCalculator.csproj", "TaxCalculator/"]
RUN dotnet restore "TaxCalculator/TaxCalculator.csproj"
COPY . .
WORKDIR "/src/TaxCalculator"
RUN dotnet publish "TaxCalculator.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaxCalculator.dll"]