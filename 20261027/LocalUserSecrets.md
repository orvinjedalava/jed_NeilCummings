# How to create a local-only secrets
Go to AuctionService folder ->
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<conn string>"

example:
Host=<your PG_SERVER_NAME>.postgres.database.azure.com;Port=5432;Database=auctions;Username=<your PG_ADMIN_USER>;Password=<your PG_ADMIN_PASSWORD>;SSL Mode=Require;Trust Server Certificate=true

# Remove user secret
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"

# Remove all user sercrets
dotnet user-secrets clear

# List user secrets
dotnet user-secrets list