#!/bin/bash
set -e

git checkout master -f
git fetch origin
git pull origin master

for b in $(git branch --format="%(refname:short)"); do
    if [[ "$b" != "master" ]]; then
        git branch -D "$b"
    fi
done
