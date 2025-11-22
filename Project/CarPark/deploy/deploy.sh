#!/bin/bash

# CarPark Project Deployment Script
# Runs on the VPS server to deploy the project
# Usage: ./deploy.sh [options]
#
# IMPORTANT: For security, create .env file on the server with database credentials
# and API keys. Do not store secrets in the repository.

set -e

# Default values
PROJECT_PATH="/opt/carpark"
MODE="minimal"
GIT_REPO=""
ENV_FILE=".env"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -p|--path)
            PROJECT_PATH="$2"
            shift 2
            ;;
        -m|--mode)
            MODE="$2"
            shift 2
            ;;
        -g|--git-repo)
            GIT_REPO="$2"
            shift 2
            ;;
        -e|--env-file)
            ENV_FILE="$2"
            shift 2
            ;;
        --help)
            echo "CarPark Project Deployment Script"
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  -p, --path PATH          Project path (default: /opt/carpark)"
            echo "  -m, --mode MODE          Deployment mode: minimal or demo (default: minimal)"
            echo "  -g, --git-repo URL       Git repository URL to clone"
            echo "  -e, --env-file FILE      Environment file path (default: .env)"
            echo "  --help                   Show this help"
            exit 0
            ;;
        *)
            log_error "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Validate mode
if [[ "$MODE" != "minimal" && "$MODE" != "demo" ]]; then
    log_error "Invalid mode: $MODE. Must be 'minimal' or 'demo'."
    exit 1
fi

log_info "Starting deployment"
log_info "Project path: $PROJECT_PATH"
log_info "Mode: $MODE"
log_info "Git repo: ${GIT_REPO:-'Not specified'}"
log_info "Environment file: $ENV_FILE"

# Check if required tools are installed, install if not
log_info "Checking required tools..."

# Update package list
sudo apt-get update

# Install curl if not present
if ! command -v curl >/dev/null 2>&1; then
    log_info "Installing curl..."
    sudo apt-get install -y curl
fi

# Install git if repo specified and git not present
if [[ -n "$GIT_REPO" ]] && ! command -v git >/dev/null 2>&1; then
    log_info "Installing git..."
    sudo apt-get install -y git
fi

# Install jq for JSON parsing
if ! command -v jq >/dev/null 2>&1; then
    log_info "Installing jq..."
    sudo apt-get install -y jq
fi

# Install Docker if not present
if ! command -v docker >/dev/null 2>&1; then
    log_info "Installing Docker..."
    curl -fsSL https://get.docker.com -o get-docker.sh && sudo sh get-docker.sh && rm get-docker.sh
    log_info "Docker installed."
else
    log_info "Docker is already installed."
fi

# Install Docker Compose if not present
if ! docker compose version >/dev/null 2>&1; then
    log_info "Installing Docker Compose..."
    sudo apt-get install -y docker-compose-plugin
else
    log_info "Docker Compose is already installed."
fi

# Create project directory
log_info "Creating project directory..."
sudo mkdir -p "$PROJECT_PATH"
sudo chown $USER:$USER "$PROJECT_PATH"

# Get project files
if [[ -n "$GIT_REPO" ]]; then
    log_info "Cloning repository..."
    if [[ -d "$PROJECT_PATH/.git" ]]; then
        log_info "Repository already exists, pulling latest changes..."
        cd "$PROJECT_PATH"
        git pull
    else
        log_info "Cloning repository with sparse checkout..."
        git clone --depth 1 --filter=blob:none --sparse "$GIT_REPO" "$PROJECT_PATH"
        cd "$PROJECT_PATH"
        git sparse-checkout set "Project/CarPark"
    fi
    # Change to project subdirectory
    cd "Project/CarPark"
else
    log_info "Using existing project files in $PROJECT_PATH"
    cd "$PROJECT_PATH"
fi

# Check if environment file exists
if [[ ! -f "$ENV_FILE" ]]; then
    log_error "Environment file not found: $ENV_FILE"
    exit 1
fi

# Deploy the application
log_info "Deploying application..."

# Stop existing containers
log_info "Stopping existing containers..."
sudo docker compose -f "docker/${MODE}-docker-compose.yml" down --volumes --remove-orphans 2>/dev/null || true

# Start the application
log_info "Starting application..."
sudo docker compose --env-file "$ENV_FILE" -f "docker/${MODE}-docker-compose.yml" up --build -d

# Wait for web service to be healthy
log_info "Waiting for web service to be healthy..."
MAX_WAIT=120
WAIT_TIME=0

while [ $WAIT_TIME -lt $MAX_WAIT ]; do
    # Check if web service is running and healthy
    WEB_STATUS=$(sudo docker compose -f "docker/${MODE}-docker-compose.yml" ps carpark-web --format "{{.Status}}" 2>/dev/null || echo "")

    if echo "$WEB_STATUS" | grep -E -q "healthy|\(healthy\)"; then
        log_info "Web service is healthy!"
        break
    elif echo "$WEB_STATUS" | grep -q "^Up" && ! echo "$WEB_STATUS" | grep -q "unhealthy"; then
        # Accept "Up" status if no healthcheck configured
        log_info "Web service is running!"
        break
    fi

    sleep 5
    WAIT_TIME=$((WAIT_TIME + 5))
    log_info "Waiting... (${WAIT_TIME}s/${MAX_WAIT}s)"
done

log_info "Deployment completed successfully!"

