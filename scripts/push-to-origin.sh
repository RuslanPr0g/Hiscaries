#!/bin/bash
set -e

if [ -z "$1" ]; then
  echo "Usage: $0 \"commit message\""
  exit 1
fi

msg="$1"

git add .
git commit -m "$msg"
git push
