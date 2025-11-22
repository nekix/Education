#!/bin/bash

# CarPark Local Deployment Script
# Manages deployment from local machine to VPS
# Usage: ./local-deploy.sh [options]

set -e

# Default values
REMOTE_HOST=""
REMOTE_USER="root"
REMOTE_PORT="22"
PROJECT_PATH="/opt/carpark"
REMOTE_SCRIPT_URL="https://raw.githubusercontent.com/nekix/Education/main/Project/CarPark/deploy/deploy.sh"

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
        -h|--host)
            REMOTE_HOST="$2"
            shift 2
            ;;
        -u|--user)
            REMOTE_USER="$2"
            shift 2
            ;;
        -p|--port)
            REMOTE_PORT="$2"
            shift 2
            ;;
        --help)
            echo "CarPark Local Deployment Script"
            echo "Usage: $0 [options]"
            echo ""
            echo "Options:"
            echo "  -h, --host HOST    Remote server IP or hostname (required)"
            echo "  -u, --user USER    SSH username (default: root)"
            echo "  -p, --port PORT    SSH port (default: 22)"
            echo "  --help             Show this help"
            echo ""
            echo "Prerequisites:"
            echo "  - SSH key authentication set up to remote server"
            echo "  - .env file exists in current directory"
            echo "  - All deployment arguments will be passed to remote script"
            echo ""
            echo "Example:"
            echo "  ./local-deploy.sh -h your-vps-ip -- -g https://github.com/user/repo.git -m demo"
            exit 0
            ;;
        --)
            # Pass remaining arguments to remote script
            shift
            REMOTE_ARGS="$*"
            break
            ;;
        *)
            log_error "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Validate required parameters
if [[ -z "$REMOTE_HOST" ]]; then
    log_error "Remote host is required. Use -h or --host to specify."
    exit 1
fi

# Check if .env file exists
if [[ ! -f ".env" ]]; then
    log_error "Local .env file not found. Please create it first."
    exit 1
fi

log_info "Starting local deployment to $REMOTE_HOST:$REMOTE_PORT as $REMOTE_USER"

# Test SSH connection
log_info "Testing SSH connection..."
if ! ssh -p "$REMOTE_PORT" -o ConnectTimeout=10 "$REMOTE_USER@$REMOTE_HOST" "echo 'SSH connection successful'" >/dev/null 2>&1; then
    log_error "Cannot connect to $REMOTE_USER@$REMOTE_HOST:$REMOTE_PORT via SSH."
    log_error "Make sure SSH authentication is set up (key or password)."
    exit 1
fi

# Copy .env file to server
log_info "Copying .env file to server..."
scp -P "$REMOTE_PORT" ".env" "$REMOTE_USER@$REMOTE_HOST:~/.env"

# Download and run deployment script on server
log_info "Running deployment on server..."
ssh -p "$REMOTE_PORT" "$REMOTE_USER@$REMOTE_HOST" "
    set -e
    log_info() { echo -e '\033[0;32m[INFO]\033[0m \$1'; }
    log_error() { echo -e '\033[0;31m[ERROR]\033[0m \$1'; }

    log_info 'Downloading deployment script...'
    wget -q '$REMOTE_SCRIPT_URL' -O deploy.sh
    chmod +x deploy.sh

    log_info 'Starting deployment...'
    ./deploy.sh -e ~/.env $REMOTE_ARGS
"

log_info "Local deployment completed!"

