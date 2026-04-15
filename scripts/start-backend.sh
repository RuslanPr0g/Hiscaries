#!/bin/bash
# Starts the Aspire backend from server/src.
# Meant to be sourced or called by other scripts.

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SERVER_DIR="$REPO_ROOT/server/src"

echo "Starting backend (Aspire)..."
(cd "$SERVER_DIR" && aspire run) &
BACKEND_PID=$!
