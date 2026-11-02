# Build images and start all services ( detached )
docker compose up --build -d

# Start only postgres ( or specific service in docker-compose.yml )
docker compose up -d postgres
docker compose up -d meilisearch

# Stop and remove containers and volumes
docker compose down -v
docker compose down -v postgres
docker compose down -v meilisearch