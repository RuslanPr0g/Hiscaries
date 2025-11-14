#!/bin/bash
set -e

git checkout master -f
git pull origin master

git fetch origin --prune

for b in $(git branch --format="%(refname:short)"); do
    if [[ "$b" != "master" ]]; then
        git branch -D "$b"
    fi
done
