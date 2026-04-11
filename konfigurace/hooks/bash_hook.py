#!/usr/bin/env python3
"""
Bash safety hook - blokuje nebezpecne prikazy (Unix + Windows)
"""
import json
import sys
import re
from pathlib import Path

# Detect PROJECT_DIR from git root (stable across CWD changes and worktrees)
import subprocess
try:
    _git_root = subprocess.run(
        ["git", "rev-parse", "--show-toplevel"],
        capture_output=True, text=True, check=True
    ).stdout.strip()
    PROJECT_DIR = Path(_git_root).resolve()
except Exception:
    PROJECT_DIR = Path(__file__).resolve().parent.parent.parent

def response(decision, reason=""):
    """Vrati spravny format odpovedi"""
    return json.dumps({
        "hookSpecificOutput": {
            "hookEventName": "PreToolUse",
            "permissionDecision": decision,
            "permissionDecisionReason": reason
        }
    })

def extract_subcommands(command):
    if not command:
        return []
    subcommands = re.split(r'\s*(?:&&|\|\||;)\s*', command)
    return [cmd.strip() for cmd in subcommands if cmd.strip()]

def is_path_in_project(path_str):
    try:
        target = Path(path_str).resolve()
        return target.is_relative_to(PROJECT_DIR)
    except (ValueError, OSError):
        return False

def check_rm_command(command):
    normalized = ' '.join(command.strip().split())
    rm_match = re.search(r'\b(rm|del|erase|rd|rmdir|Remove-Item)\b', normalized, re.IGNORECASE)
    if not rm_match:
        return False, None
    if re.search(r'(^|[\s"])(/|~|[A-Z]:\\?)($|[\s"])', normalized):
        return True, "Mazani root path zakazano"
    parts = normalized.split()
    for part in parts:
        if part.startswith('-') or part.startswith('/'):
            continue
        if re.match(r'^(rm|del|erase|rd|rmdir|Remove-Item)$', part, re.IGNORECASE):
            continue
        if re.match(r'^[A-Za-z]:\\', part) or part.startswith('/'):
            if not is_path_in_project(part):
                return True, f"Mazani mimo projekt zakazano: {part}"
    return False, None

DANGEROUS_PATTERNS = [
    (r'\.env\b', ".env pristup zakazan"),
    (r'git\s+push\s+.*(--force|--force-with-lease|-f|\s\+)', "git force push zakazano"),
    (r'git\s+reset\s+--hard', "git reset --hard zakazano"),
    (r'git\s+checkout\s+\.', "git checkout . zakazano"),
    (r'git\s+checkout\s+(-f|--force)', "git checkout -f zakazano"),
    (r'git\s+clean', "git clean zakazano"),
    (r'git\s+restore\s+\.', "git restore . zakazano"),
    (r'git\s+branch\s+-D', "git branch -D zakazano"),
    # Branch create - tvrdy blok
    (r'git\s+checkout\s+-b\b', "git checkout -b zakazano - pracujeme jen na main"),
    (r'git\s+branch\s+(?!-D\b)(?!-d\b)(?!-a\b)(?!-r\b)(?!-v\b)(?!--list\b)\S', "git branch create zakazano"),
    (r'git\s+switch\s+(-c|--create)\b', "git switch -c zakazano - pracujeme jen na main"),
    # PowerShell
    (r'\bgc\s+', "gc zakazano"),
    (r'Set-Content\s+', "Set-Content zakazano"),
    (r'Out-File\s+', "Out-File zakazano"),
    (r'Clear-Content\s+', "Clear-Content zakazano"),
    # Systemove
    (r'\bformat\s+[a-zA-Z]:', "format disku zakazano"),
    (r'DROP\s+DATABASE', "DROP DATABASE zakazano"),
    (r'TRUNCATE\s+TABLE', "TRUNCATE TABLE zakazano"),
    (r'\bdd\s+', "dd zakazano"),
    (r'\bmkfs', "mkfs zakazano"),
    (r'cmd\s+/c\s+.*rm', "cmd /c rm zakazano"),
    (r'powershell\s+-EncodedCommand', "EncodedCommand zakazano"),
    (r'powershell\s+-e\s+', "powershell -e zakazano"),
]

ASK_PATTERNS = [
    (r'git\s+commit', "git commit - potvrzeni uzivatele"),
    (r'git\s+push\b(?!.*(?:--force|--force-with-lease|-f))', "git push - potvrzeni uzivatele"),
]

def check_command(command):
    """Returns (decision, reason) where decision is 'deny', 'ask', or None (allow)."""
    normalized = ' '.join(command.strip().split())
    blocked, reason = check_rm_command(normalized)
    if blocked:
        return "deny", reason
    for pattern, reason in DANGEROUS_PATTERNS:
        if re.search(pattern, normalized, re.IGNORECASE):
            return "deny", reason
    for pattern, reason in ASK_PATTERNS:
        if re.search(pattern, normalized, re.IGNORECASE):
            return "ask", reason
    return None, None

def main():
    try:
        data = json.load(sys.stdin)
    except json.JSONDecodeError:
        print(response("ask", "JSON parse error"))
        sys.exit(0)

    permission_mode = data.get("permission_mode", "default")

    if data.get("tool_name") != "Bash":
        print(response("allow"))
        sys.exit(0)

    command = data.get("tool_input", {}).get("command", "")

    ask_reasons = []
    for subcmd in extract_subcommands(command):
        decision, reason = check_command(subcmd)
        if decision == "deny":
            print(response("deny", f"BLOKOVANO: {reason}\n\nPrikaz: {subcmd}"))
            sys.exit(0)
        if decision == "ask":
            ask_reasons.append(reason)

    if ask_reasons:
        print(response("ask", " | ".join(ask_reasons)))
        sys.exit(0)

    print(response("allow"))
    sys.exit(0)

if __name__ == "__main__":
    main()
