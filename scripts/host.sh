#!/bin/bash
# Starts the full app in host/network mode (e.g. for LAN access).
# Backend: aspire run (from server/src)
# Frontend: npm run host (binds to 0.0.0.0, no watch)
set -e

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

source "$REPO_ROOT/scripts/start-backend.sh"

echo "Starting frontend (npm run host)..."
npm run host --prefix "$REPO_ROOT/client" &
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
