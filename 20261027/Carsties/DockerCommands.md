# Build images and start all services ( detached )
docker compose up --build -d

# Start only postgres ( or specific service in docker-compose.yml )
docker compose up -d postgres

# Stop and remove containers and volumes
docker compose down -v