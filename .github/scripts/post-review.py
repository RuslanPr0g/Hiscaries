#!/usr/bin/env python3
"""
Reads Claude's findings JSON and posts a single PR review with inline comments.
Usage: post-review.py --findings /tmp/review.json --repo owner/repo --pr 42
"""
import argparse, json, subprocess, sys

SEVERITY_EMOJI = {"critical": "🔴", "high": "🟠", "medium": "🟡"}


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--findings", required=True)
    parser.add_argument("--repo", required=True)
    parser.add_argument("--pr", required=True, type=int)
    args = parser.parse_args()

    with open(args.findings) as f:
        data = json.load(f)

    findings = data.get("findings", [])
    if not findings:
        print("No findings — skipping review comment.")
        return

    comments = []
    for finding in findings:
        emoji = SEVERITY_EMOJI.get(finding["severity"], "⚪")
        body = f"{emoji} **[{finding['severity'].upper()}] {finding['title']}**\n\n{finding['body']}"
        comments.append({"path": finding["file"], "line": finding["line"], "body": body})

    counts = {"critical": 0, "high": 0, "medium": 0}
    for finding in findings:
        counts[finding["severity"]] += 1

    summary_lines = ["## AI Code Review\n"]
    if counts["critical"]:
        summary_lines.append(f"🔴 **{counts['critical']} critical**")
    if counts["high"]:
        summary_lines.append(f"🟠 **{counts['high']} high**")
    if counts["medium"]:
        summary_lines.append(f"🟡 {counts['medium']} medium")
    review_body = "\n".join(summary_lines)

    event = "REQUEST_CHANGES" if (counts["critical"] or counts["high"]) else "COMMENT"

    owner, repo = args.repo.split("/", 1)
    payload = json.dumps({"body": review_body, "event": event, "comments": comments})

    result = subprocess.run(
        ["gh", "api", f"/repos/{owner}/{repo}/pulls/{args.pr}/reviews",
         "--method", "POST", "--input", "-"],
        input=payload, text=True, capture_output=True
    )

    if result.returncode != 0:
        print(result.stderr, file=sys.stderr)
        sys.exit(1)

    print(f"Posted review: {event} with {len(findings)} finding(s).")

    if counts["critical"] or counts["high"]:
        print(f"::error::{counts['critical']} critical, {counts['high']} high severity finding(s) "
              f"must be resolved before this PR can be merged.", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
