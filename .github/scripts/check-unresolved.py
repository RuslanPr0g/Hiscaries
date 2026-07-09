#!/usr/bin/env python3
"""
Fails if the PR has unresolved review threads left over from a previous AI
review run at critical or high severity. Runs before the new Claude analysis
so a PR with outstanding blocking feedback fails fast, without spending a
Claude call, until those threads are resolved.

Usage: check-unresolved.py --repo owner/repo --pr 42
"""
import argparse, os, sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from review_threads import fetch_review_threads, bot_authored

# Must match SEVERITY_EMOJI["critical"] / ["high"] in post-review.py
BLOCKING_PREFIXES = ("🔴", "🟠")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo", required=True)
    parser.add_argument("--pr", required=True, type=int)
    args = parser.parse_args()

    owner, repo = args.repo.split("/", 1)
    threads = fetch_review_threads(owner, repo, args.pr)

    blocking = []
    for thread in threads:
        if thread["isResolved"] or not bot_authored(thread):
            continue
        body = thread["comments"]["nodes"][0]["body"]
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
