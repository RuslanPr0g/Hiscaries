#!/bin/bash
# Starts the full app for local development.
# Backend: aspire run (from server/src)
# Frontend: npm run start (proxied to localhost)
set -e

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

source "$REPO_ROOT/scripts/start-backend.sh"

echo "Starting frontend (npm run start)..."
npm run start --prefix "$REPO_ROOT/client" &
FRONTEND_PID=$!

cleanup() {
  echo ""
  echo "Shutting down..."
  kill "$BACKEND_PID" "$FRONTEND_PID" 2>/dev/null
  wait "$BACKEND_PID" "$FRONTEND_PID" 2>/dev/null
  echo "Done."
}

trap cleanup SIGINT SIGTERM

echo ""
echo "Both services are starting. Press Ctrl+C to stop."
wait
