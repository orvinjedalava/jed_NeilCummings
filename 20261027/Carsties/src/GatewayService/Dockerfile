# Stage 1: build with the full SDK image (large, not shipped)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the csproj first so the restore layer is cached until dependencies change
COPY GatewayService/GatewayService.csproj GatewayService/
RUN dotnet restore GatewayService/GatewayService.csproj

COPY GatewayService/ GatewayService/
RUN dotnet publish GatewayService/GatewayService.csproj -c Release -o /app --no-restore

# Stage 2: runtime-only image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Make Kestrel listen on 8080 (non-root images can't bind to port 80)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GatewayService.dll"]