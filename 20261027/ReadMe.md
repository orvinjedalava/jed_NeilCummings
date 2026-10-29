# Git Repo
github.com/TryCatchLearn/carsties

# Udemy
https://www.udemy.com/course/build-a-microservices-app-with-dotnet-and-nextjs-from-scratch/learn/lecture/57936549#overview

# Steps to deploy to Azure

1. Create dockerfile and dockercompose files to deploy AuctionService and PostGreSQL as containers in same network

how to run in dev?
docker compose up --build

2. Provision "Azure Database for PostgreSQL - Flexible Server" in Azure

Run the **Carsties Provision PostgreSQL** workflow with the `admin_user` input
(default: `carstiesadminuser`). This replaces the `PG_ADMIN_USER` repository
variable. Use 1-63 letters and numbers, starting with a letter; hyphens such as
those in `carsties-admin-user` are not accepted by Azure. Use the same username
in the AuctionService connection string. For an existing server, supply its
current admin username because Azure does not allow it to be changed.

3. Provision new Container App for AuctionService

4. Create Secret and Env Variable for Container App to connect Auction Service with PostgreSQL

# Steps from Copilot
Your assumption is correct: the database should come first, because the Container App needs its connection string at deploy time.

1. Provision an Azure Database for PostgreSQL Flexible Server, plus a database (e.g. auctions) and an admin user.

2. Set the server's networking: allow Azure services (or use a VNet/private access) and add your own IP for local access.

3. Build the connection string for the Azure database and test it from your machine, for example by running the app locally or running EF migrations against it.

4. Create a Container Apps environment (and a resource group, if you don't have one).

5. Push the AuctionService image to a registry (Azure Container Registry or Docker Hub), using the existing Dockerfile.

6. Create the Container App from that image, with ingress enabled on the port the app listens on.
7. Store the connection string as a Container App secret and expose it as an env variable, e.g. ConnectionStrings__DefaultConnection, referencing the secret.

8. Make sure the schema exists in Azure, either through the startup migration/DbInitializer or by running migrations manually, then call the API to verify.
9. Later, automate steps 5–7 with a GitHub Actions workflow (build image, push, update Container App).

Correction to the ordering: steps 4 and 5 don't depend on the database, so they can happen in any order relative to step 1. Only step 7 needs the database to exist first.

# How to get my Public IP?
curl -s https://api.ipify.org

# How to create a local-only secrets
Go to AuctionService folder ->
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<conn string>"