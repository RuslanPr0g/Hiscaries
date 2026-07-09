#!/usr/bin/env python3
"""Shared GraphQL helper for reading a PR's review threads. REST has no
concept of thread resolution — only GraphQL exposes `isResolved`."""
import json, subprocess, sys

QUERY = """
query($owner: String!, $repo: String!, $pr: Int!, $after: String) {
  repository(owner: $owner, name: $repo) {
    pullRequest(number: $pr) {
      reviewThreads(first: 100, after: $after) {
        pageInfo { hasNextPage endCursor }
        nodes {
          isResolved
          path
          line
          comments(first: 1) {
            nodes { body author { login } }
          }
        }
      }
    }
  }
}
"""


def fetch_review_threads(owner, repo, pr):
    threads = []
    after = None
    while True:
        args = [
            "gh", "api", "graphql",
            "-f", f"query={QUERY}",
            "-f", f"owner={owner}",
            "-f", f"repo={repo}",
            "-F", f"pr={pr}",
        ]
        args += ["-f", f"after={after}"] if after else ["-F", "after=null"]

        result = subprocess.run(args, capture_output=True, text=True)
        if result.returncode != 0:
            print(result.stderr, file=sys.stderr)
            sys.exit(1)

        page = json.loads(result.stdout)["data"]["repository"]["pullRequest"]["reviewThreads"]
        threads.extend(page["nodes"])
        if not page["pageInfo"]["hasNextPage"]:
            break
        after = page["pageInfo"]["endCursor"]
    return threads


def bot_authored(thread):
    comments = thread["comments"]["nodes"]
    if not comments:
        return False
    author = (comments[0].get("author") or {}).get("login", "")
    return "github-actions" in author
