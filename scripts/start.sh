#!/bin/bash

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

cleanup() {
  echo ""
  echo "Shutting down..."
  pkill -P $BACKEND_PID 2>/dev/null
  pkill -P $FRONTEND_PID 2>/dev/null
  kill "$BACKEND_PID" "$FRONTEND_PID" 2>/dev/null
  echo "Done."
  exit 0
}

trap cleanup SIGINT SIGTERM

curl -sSL https://aspire.dev/install.sh | bash

echo "Starting backend (Aspire)..."
(cd "$REPO_ROOT/server/src" && aspire run) &
BACKEND_PID=$!

echo "Starting frontend..."
(cd "$REPO_ROOT/client" && npm run start) &
FRONTEND_PID=$!

# wait in a loop so trap can fire
while kill -0 "$BACKEND_PID" 2>/dev/null || kill -0 "$FRONTEND_PID" 2>/dev/null; do
  sleep 1
done
