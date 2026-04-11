#!/usr/bin/env python3
"""
Stop hook - kontroluje dokončení úkolů jen v acceptEdits modu.
V normálním modu vždy povolí zastavení.
"""
import json
import sys
import subprocess

def main():
    try:
        data = json.load(sys.stdin)
    except json.JSONDecodeError:
        # Nelze parsovat → povolit zastavení
        print(json.dumps({"decision": "approve"}))
        sys.exit(0)

    permission_mode = data.get("permission_mode", "default")

    # V normálním modu vždy povolit zastavení
    if permission_mode != "acceptEdits":
        print(json.dumps({"decision": "approve"}))
        sys.exit(0)

    # Pokud už běží stop hook (prevence smyčky)
    if data.get("stop_hook_active"):
        print(json.dumps({"decision": "approve"}))
        sys.exit(0)

    # V acceptEdits modu — spustit AI kontrolu
    # Vrátíme "block" s důvodem, Claude to dostane jako instrukci
    # Necháme to na hlavním promptu - jen připomeneme ať zkontroluje
    print(json.dumps({"decision": "block", "reason": "Before stopping, verify all requested tasks are complete. If everything is done, say so and stop."}))
    sys.exit(0)

if __name__ == "__main__":
    main()
