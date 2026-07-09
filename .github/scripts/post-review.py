#!/usr/bin/env python3
"""
Reads Claude's findings JSON and posts a single PR review with inline comments.
Usage: post-review.py --findings /tmp/review.json --diff /tmp/pr.diff --repo owner/repo --pr 42
"""
import argparse, json, re, subprocess, sys

SEVERITY_EMOJI = {"critical": "🔴", "high": "🟠", "medium": "🟡"}

HUNK_HEADER_RE = re.compile(r"^@@ -\d+(?:,\d+)? \+(\d+)(?:,\d+)? @@")


def parse_commentable_lines(diff_text):
    """GitHub only accepts inline review comments on lines visible in the diff
    (added or context lines within a hunk) — anything else gets the whole
    review batch rejected with HTTP 422. Build the set of (file, line) pairs
    that are actually safe to comment on, per the new-file line numbering
    Claude is instructed to report."""
    commentable = {}
    current_file = None
    new_line = None
    for line in diff_text.splitlines():
        if line.startswith("+++ "):
            path = line[4:].strip()
            current_file = path[2:] if path.startswith("b/") else (None if path == "/dev/null" else path)
            new_line = None
            continue
        if line.startswith("@@"):
            match = HUNK_HEADER_RE.match(line)
            new_line = int(match.group(1)) if (match and current_file) else None
            continue
        if current_file is None or new_line is None:
            continue
        if line.startswith("+") and not line.startswith("+++"):
            commentable.setdefault(current_file, set()).add(new_line)
            new_line += 1
        elif line.startswith(" "):
            commentable.setdefault(current_file, set()).add(new_line)
            new_line += 1
        # lines starting with "-" have no line on the right side — don't advance
    return commentable


def format_finding(finding, with_location=False):
    emoji = SEVERITY_EMOJI.get(finding["severity"], "⚪")
    location = f" `{finding['file']}:{finding['line']}`" if with_location else ""
    return f"{emoji} **[{finding['severity'].upper()}]{location} {finding['title']}**\n\n{finding['body']}"


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--findings", required=True)
    parser.add_argument("--diff", required=True)
    parser.add_argument("--repo", required=True)
    parser.add_argument("--pr", required=True, type=int)
    args = parser.parse_args()

    with open(args.findings) as f:
        findings = json.load(f).get("findings", [])
    if not findings:
        print("No findings — skipping review comment.")
        return

    with open(args.diff) as f:
        commentable = parse_commentable_lines(f.read())

    inline_comments, folded = [], []
    for finding in findings:
        if finding["line"] in commentable.get(finding["file"], set()):
            inline_comments.append({"path": finding["file"], "line": finding["line"], "body": format_finding(finding)})
        else:
            folded.append(finding)

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

    if folded:
        summary_lines.append(
            "\n### Findings outside the diff\n"
            "GitHub only allows inline comments on lines visible in the diff, so these are listed here instead:\n"
        )
        for finding in folded:
            summary_lines.append(f"\n{format_finding(finding, with_location=True)}")

    event = "REQUEST_CHANGES" if (counts["critical"] or counts["high"]) else "COMMENT"

    owner, repo = args.repo.split("/", 1)
    payload = json.dumps({"body": "\n".join(summary_lines), "event": event, "comments": inline_comments})

    result = subprocess.run(
        ["gh", "api", f"/repos/{owner}/{repo}/pulls/{args.pr}/reviews",
         "--method", "POST", "--input", "-"],
        input=payload, text=True, capture_output=True
    )

    if result.returncode != 0:
        print(result.stderr, file=sys.stderr)
        sys.exit(1)

    print(f"Posted review: {event} with {len(inline_comments)} inline, {len(folded)} folded into summary.")

    if counts["critical"] or counts["high"]:
        print(f"::error::{counts['critical']} critical, {counts['high']} high severity finding(s) "
              f"must be resolved before this PR can be merged.", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
