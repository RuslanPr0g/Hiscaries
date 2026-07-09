#!/usr/bin/env python3
"""
Fails if the PR has unresolved review threads left over from a previous AI
review run at critical or high severity. Runs before the new Claude analysis
so a PR with outstanding blocking feedback fails fast, without spending a
Claude call, until those threads are resolved.

Usage: check-unresolved.py --repo owner/repo --pr 42
"""
import argparse, json, subprocess, sys

# Must match SEVERITY_EMOJI["critical"] / ["high"] in post-review.py
BLOCKING_PREFIXES = ("🔴", "🟠")

QUERY = """
query($owner: String!, $repo: String!, $pr: Int!, $after: String) {
  repository(owner: $owner, name: $repo) {
    pullRequest(number: $pr) {
      reviewThreads(first: 100, after: $after) {
        pageInfo { hasNextPage endCursor }
        nodes {
          isResolved
          comments(first: 1) {
            nodes { body author { login } }
          }
        }
      }
    }
  }
}
"""


def fetch_threads(owner, repo, pr):
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


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo", required=True)
    parser.add_argument("--pr", required=True, type=int)
    args = parser.parse_args()

    owner, repo = args.repo.split("/", 1)
    threads = fetch_threads(owner, repo, args.pr)

    blocking = []
    for thread in threads:
        if thread["isResolved"]:
            continue
        comments = thread["comments"]["nodes"]
        if not comments:
            continue
        first = comments[0]
        author = (first.get("author") or {}).get("login", "")
        body = first.get("body", "")
        if "github-actions" not in author:
            continue
        if body.startswith(BLOCKING_PREFIXES):
            blocking.append(body.splitlines()[0])

    if blocking:
        print(f"::error::{len(blocking)} unresolved critical/high finding(s) from a previous AI review "
              f"must be resolved (or marked resolved) before this PR can proceed:")
        for line in blocking:
            print(f"  - {line}")
        sys.exit(1)

    print("No unresolved critical/high AI review findings from previous runs.")


if __name__ == "__main__":
    main()
