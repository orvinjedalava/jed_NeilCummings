# Git Repo
github.com/TryCatchLearn/carsties

# Udemy
https://www.udemy.com/course/build-a-microservices-app-with-dotnet-and-nextjs-from-scratch/learn/lecture/57936549#overview

# Steps to deploy to Azure

1. Create dockerfile and dockercompose files to deploy AuctionService and PostGreSQL as containers in same network

how to run in dev?
docker compose up --build

2. Provision "Azure Database for PostgreSQL - Flexible Server" in Azure

3. Provision new Container App for AuctionService

4. Create Secret and Env Variable for Container App to connect Auction Service with PostgreSQL