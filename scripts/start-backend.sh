#!/bin/bash
set -e

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

curl -sSL https://aspire.dev/install.sh | bash

echo "Starting backend (Aspire)..."
(cd "$REPO_ROOT/server/src" && aspire run) &
BACKEND_PID=$!
