# Build stage
FROM sh-artifactory.sh.shaam.gov.il:8082/docker/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy Porject File
COPY . .

RUN ls -R /app

# Configure Repo
RUN dotnet nuget add source http://sh-artifactory.sh.shaam.gov.il:8082/artifactory/api/nuget/v3/nuget -n artifactory
RUN dotnet nuget disable source nuget.org
# RUN dotnet restore --interactive

# Build Project
ENV NUGET_CERT_REVOCATION_MODE=offline
RUN dotnet publish -c=Release -o=out

# Test
RUN dotnet test TaxCalculator.Tests/TaxCalculator.Tests.csproj

# Runtime stage
FROM sh-artifactory.sh.shaam.gov.il:8082/docker/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy Artifact
COPY --from=build /app/out ./

# Copy only the TaxCalculator.csproj file to the container's /app directory
COPY TaxCalculator/TaxCalculator.csproj ./

# Expose Port
EXPOSE 8080

# Run Project
ENTRYPOINT ["dotnet", "TaxCalculator.dll"]
