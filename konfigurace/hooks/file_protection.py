#!/usr/bin/env python3
"""
File protection hook - blokuje editace mimo projekt a chranenych souboru
"""
import json
import sys
import os
import re
from pathlib import Path

# Detect PROJECT_DIR from git root (stable across CWD changes)
import subprocess
try:
    _git_root = subprocess.run(
        ["git", "rev-parse", "--show-toplevel"],
        capture_output=True, text=True, check=True
    ).stdout.strip()
    PROJECT_DIR = Path(_git_root).resolve()
except Exception:
    PROJECT_DIR = Path(os.getcwd()).resolve()

# Dynamicke cesty podle OS
_home = Path.home()
_temp = Path(os.environ.get("TEMP", "/tmp")).resolve()
ALLOWED_PATHS = [
    (_home / ".claude" / "plans").resolve(),
    (_home / ".claude").resolve(),
    _temp,
]

PROTECTED_PATTERNS = [
    r'\.env$',
    r'\.env\.',
    r'^mcp\.json$',
    r'^settings\.json$',
    r'^settings\.local\.json$',
    r'CLAUDE\.md$',
]

def response(decision, reason=""):
    """Vrati spravny format odpovedi"""
    return json.dumps({
        "hookSpecificOutput": {
            "hookEventName": "PreToolUse",
            "permissionDecision": decision,
            "permissionDecisionReason": reason
        }
    })

def is_path_in_project(file_path):
    try:
        target = Path(file_path).resolve()
        return target.is_relative_to(PROJECT_DIR)
    except (ValueError, OSError):
        return False

def is_path_in_allowed(file_path):
    try:
        target = Path(file_path).resolve()
        for allowed in ALLOWED_PATHS:
            if target.is_relative_to(allowed):
                return True
        return False
    except (ValueError, OSError):
        return False

def main():
    try:
        data = json.load(sys.stdin)
    except json.JSONDecodeError:
        print(response("ask", "JSON parse error"))
        sys.exit(0)

    permission_mode = data.get("permission_mode", "default")

    tool_name = data.get("tool_name")
    if tool_name not in ("Edit", "Write"):
        if permission_mode in ("acceptEdits", "plan"):
            print(response("allow"))
        else:
            print(response("ask", f"Tool: {tool_name}"))
        sys.exit(0)

    file_path = data.get("tool_input", {}).get("file_path", "")
    filename = os.path.basename(file_path)

    # Povolena cesta mimo projekt
    if is_path_in_allowed(file_path):
        if permission_mode in ("acceptEdits", "plan"):
            print(response("allow"))
        else:
            print(response("ask", f"File: {filename}"))
        sys.exit(0)

    # Mimo projekt
    if not is_path_in_project(file_path):
        print(response("deny", f"BLOKOVANO: Editace mimo projekt\n\nSoubor: {file_path}"))
        sys.exit(0)

    # Chraneny soubor
    for pattern in PROTECTED_PATTERNS:
        if re.search(pattern, filename, re.IGNORECASE):
            if permission_mode in ("acceptEdits", "plan"):
                print(response("deny", f"BLOKOVANO: Chraneny soubor\n\nSoubor: {filename}"))
            else:
                print(response("ask", f"Chraneny soubor: {filename}"))
            sys.exit(0)

    # OK
    if permission_mode in ("acceptEdits", "plan"):
        print(response("allow"))
    else:
        print(response("ask", f"File: {filename}"))
    sys.exit(0)

if __name__ == "__main__":
    main()
