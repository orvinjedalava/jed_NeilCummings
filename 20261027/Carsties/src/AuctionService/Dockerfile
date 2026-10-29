# Stage 1: build with the full SDK image (large, not shipped)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the csproj first so the restore layer is cached until dependencies change
COPY AuctionService.csproj .
RUN dotnet restore

# Copy the rest of the source; changes here don't invalidate the restore layer
COPY . .
# Compile and output deployable files to /app; --no-restore reuses the earlier restore
RUN dotnet publish -c Release -o /app --no-restore

# Stage 2: runtime-only image (much smaller, no SDK)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Take only the published output from the build stage
COPY --from=build /app .

# Make Kestrel listen on 8080 (non-root images can't bind to port 80)
ENV ASPNETCORE_HTTP_PORTS=8080
# Documentation only; the actual mapping is done by compose or `docker run -p`
EXPOSE 8080

# Start the app when the container launches
ENTRYPOINT ["dotnet", "AuctionService.dll"]