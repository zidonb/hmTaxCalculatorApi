# Use official .NET runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY TaxCalculator.csproj ./
RUN dotnet restore "TaxCalculator.csproj"

# Copy the rest of the application
COPY . .  
WORKDIR /src
RUN dotnet publish "TaxCalculator.csproj" -c Release -o /app/publish

# Final stage: Run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaxCalculator.dll"]
