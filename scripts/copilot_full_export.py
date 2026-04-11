#!/usr/bin/env python3
"""Export Copilot konverzace z Nitrite DB — chronologicky, kompletní file operace.

Strategy:
- File ops (creates, patches, reads, searches): byte position works for ordering
- Terminals: assigned to iterations by content heuristics (byte positions unreliable)
- Infrastructure debugging (kill, lsof, pgrep) filtered out
"""
import re, sys, os, glob


def shorten_path(path):
    """Remove long prefix, keep from project root."""
    for marker in ['/copilot/', '/claude-code/', '/chatgpt/', '/baseline/']:
        if marker in path:
            return path.split(marker, 1)[1]
    return path


def normalize(s):
    """Strip backslashes and whitespace for dedup."""
    return re.sub(r'[\\\s]+', ' ', s).strip()


def main():
    db_path = sys.argv[1] if len(sys.argv) > 1 else None
    out_path = sys.argv[2] if len(sys.argv) > 2 else 'konverzace.md'

    if not db_path:
        sessions = glob.glob(os.path.expanduser(
            '~/.config/github-copilot/rd/chat-agent-sessions/*/copilot-agent-sessions-nitrite.db'))
        if not sessions:
            print('No sessions found'); return
        db_path = max(sessions, key=os.path.getmtime)
        print(f'Using: {db_path}')

    with open(db_path, 'rb') as f:
        raw = f.read()
    text = raw.decode('utf-8', errors='replace')

    # ═══ 1. USER MESSAGES ═══
    user_msgs = []
    seen_msgs = set()
    for m in re.finditer(rb'stringContent\x74([\x00-\xff]{2})', raw):
        pos = m.end()
        length = int.from_bytes(m.group(1), 'big')
        if 5 < length < 60000:
            msg = raw[pos:pos + length].decode('utf-8', errors='replace').strip()
            if msg and msg not in seen_msgs:
                seen_msgs.add(msg)
                user_msgs.append((m.start(), msg))

    if not user_msgs:
        print('No user messages found'); return

    user_positions = [pos for pos, _ in user_msgs]
    num_iters = len(user_msgs)

    def get_iter(byte_pos):
        it = 0
        for up in user_positions:
            if byte_pos > up:
                it += 1
        return max(it, 1)

    # ═══ 2. FILE OPS (byte position reliable for these) ═══
    file_ops = []  # (pos, type, detail)

    seen_c = set()
    for m in re.finditer(r'Created \[([^\]]+)\]\(file:///([^)]+)\)', text):
        path = shorten_path(m.group(2))
        if path not in seen_c:
            seen_c.add(path)
            file_ops.append((m.start(), 'create_file', path))

    seen_p = set()
    for m in re.finditer(r'Edited \[([^\]]+)\]\(file:///([^)]+)\)', text):
        path = shorten_path(m.group(2))
        if path not in seen_p:
            seen_p.add(path)
            file_ops.append((m.start(), 'apply_patch', path))

    seen_r = set()
    for m in re.finditer(r'Read file \[([^\]]+)\]\(file:///([^)]+)\)', text):
        path = shorten_path(m.group(2))
        if path not in seen_r:
            seen_r.add(path)
            file_ops.append((m.start(), 'read_file', path))

    seen_s = set()
    for m in re.finditer(r'Searched for files matching query: ([^\n"]{3,100})', text):
        q = m.group(1).strip().rstrip('\\').rstrip()
        nq = normalize(q)
        if nq not in seen_s:
            seen_s.add(nq)
            file_ops.append((m.start(), 'file_search', q))

    file_ops.sort(key=lambda x: x[0])

    # Group file ops by iteration
    iter_file_ops = {i: [] for i in range(1, num_iters + 1)}
    for pos, op_type, detail in file_ops:
        it = get_iter(pos)
        if 1 <= it <= num_iters:
            iter_file_ops[it].append((op_type, detail))

    # ═══ 3. TERMINALS (content-based iteration assignment) ═══
    utext = raw.decode('utf-8', errors='replace')
    for _ in range(6):
        utext = utext.replace('\\\\', '\\')
    utext = utext.replace('\\"', '"').replace('\\u0026', '&').replace('\\u0027', "'")
    utext = utext.replace('\\u003c', '<').replace('\\u003d', '=').replace('\\u003e', '>')

    all_cmds = []
    seen_t = set()
    for m in re.finditer(r'"command"\s*:\s*"([^"]{5,500})"', utext):
        cmd = m.group(1).replace('\n', ' ').strip()[:200]
        if cmd and not cmd.startswith('{') and not cmd.startswith('command'):
            k = cmd[:80]
            if k not in seen_t:
                seen_t.add(k)
                all_cmds.append(cmd)

    # Filter out infrastructure debugging
    infra_kw = ['kill ', 'lsof ', 'pgrep ', 'docker compose stop', 'docker rm']
    all_cmds = [c for c in all_cmds if not any(kw in c for kw in infra_kw)]

    # Assign terminals to iterations by content heuristics
    # Iter 1: initial build verification (git status, dotnet test, npm build on standard port or no port)
    # Iter 2: implementation + build (dotnet test after changes, npm build, git status)
    # Iter 3: E2E testing (curl API calls, stripe trigger/events, webhook payloads, HMAC)
    iter_terminals = {i: [] for i in range(1, num_iters + 1)}

    # Heuristic: terminals that do API calls (curl with auth), stripe trigger/events,
    # HMAC, webhook payloads → iter 3 (testing)
    test_kw = ['curl -', 'stripe trigger', 'stripe events', 'hmac', 'webhook_',
               '/api/auth/login', '/api/bookings', '/api/webhooks',
               '/tmp/evt_', '/tmp/trigger', 'echo a', 'echo READY', 'echo CLEAN',
               'set -e\n', 'cat /tmp/']

    # Heuristic: build/verify commands → iter where implementation happens
    build_kw = ['dotnet test', 'npm run build', 'npm install', 'dotnet build', 'dotnet run']

    # Heuristic: setup commands → iter 1 or 2
    setup_kw = ['docker compose up', 'docker run ', 'stripe --version', 'stripe listen',
                'git --no-pager']

    # First pass: categorize
    test_cmds = []
    build_cmds = []
    setup_cmds = []
    other_cmds = []

    for cmd in all_cmds:
        if any(kw in cmd for kw in test_kw):
            test_cmds.append(cmd)
        elif any(kw in cmd for kw in build_kw):
            build_cmds.append(cmd)
        elif any(kw in cmd for kw in setup_kw):
            setup_cmds.append(cmd)
        else:
            other_cmds.append(cmd)

    # Assign: if there are 3 iterations with pattern prompt/continue/test:
    if num_iters >= 3:
        # Iter 1 gets: first build commands (up to 3) + first setup commands
        build_for_1 = build_cmds[:3] if build_cmds else []
        remaining_build = build_cmds[3:] if len(build_cmds) > 3 else []

        iter_terminals[1] = setup_cmds[:3] + build_for_1
        iter_terminals[2] = setup_cmds[3:] + remaining_build + other_cmds
        iter_terminals[num_iters] = test_cmds
    elif num_iters == 2:
        iter_terminals[1] = setup_cmds + build_cmds
        iter_terminals[2] = test_cmds + other_cmds
    else:
        iter_terminals[1] = all_cmds

    # ═══ 4. CHECKLIST + REPLIES (deduplicated) ═══
    seen_ch = set()
    checks = []
    for m in re.finditer(r'- \[(x| )\] ([^\n"]{5,200})', text):
        item = m.group(2).strip()
        ni = normalize(item)
        if ni not in seen_ch:
            seen_ch.add(ni)
            checks.append((get_iter(m.start()), f'- [{m.group(1)}] {item}'))

    seen_rep = set()
    replies = []
    for m in re.finditer(r'(?:Hotovo|Implementace je hotová|Pokračuju|Navazuju)[^\n"]{5,300}', text):
        rep = m.group(0)[:200]
        nr = normalize(rep[:60])
        if nr not in seen_rep:
            seen_rep.add(nr)
            replies.append((get_iter(m.start()), rep))

    # ═══ 5. BUILD OUTPUT ═══
    lines = ['# Copilot Agent Session — kompletní konverzace', '',
             f'DB: `{db_path}`', '', '', '---', '']

    tc = tp = tt = 0
    for i in range(1, num_iters + 1):
        _, msg = user_msgs[i - 1]
        lines.append(f'## Iterace {i} — Uživatel')
        lines.append('')
        if len(msg) > 1500:
            lines.append(msg[:300] + f'\n\n*[...zkráceno, {len(msg)} znaků...]*\n\n' + msg[-200:])
        else:
            lines.append(msg)
        lines.append('')
        lines.append(f'### Copilot (iterace {i})')
        lines.append('')

        ic = ip = it_count = 0
        ro_kw = ['dotnet test', 'curl ', 'cat ', 'head ', 'echo ', 'python3 -c', 'python3 -']

        # File ops
        for op_type, detail in iter_file_ops[i]:
            if op_type == 'file_search':
                lines.append(f'> **file_search**: {detail}')
            elif op_type == 'read_file':
                lines.append(f'> **read_file**: `{detail}`')
            elif op_type == 'create_file':
                lines.append(f'> **create_file**: `{detail}`')
                ic += 1
            elif op_type == 'apply_patch':
                lines.append(f'> **apply_patch**: `{detail}`')
                ip += 1

        # Checks for this iter
        for it, check in checks:
            if it == i:
                lines.append(check)

        # Replies for this iter
        for it, rep in replies:
            if it == i:
                lines.append(rep)

        # Terminals
        for cmd in iter_terminals.get(i, []):
            marker = 'RO' if any(k in cmd for k in ro_kw) else 'SC'
            lines.append(f'> **terminal [{marker}]**: `{cmd}`')
            it_count += 1

        lines.append('')
        lines.append(f'> **Souhrn iterace {i}:** {ic} creates, {ip} patches, {it_count} terminals')
        lines.append('')
        lines.append('---')
        lines.append('')
        tc += ic; tp += ip; tt += it_count

    # Summary table
    lines.extend(['## Souhrn', '',
                  '| Iterace | Creates | Patches | Terminals |',
                  '|---------|---------|---------|-----------|'])
    for i in range(1, num_iters + 1):
        c = sum(1 for t, _ in iter_file_ops[i] if t == 'create_file')
        p = sum(1 for t, _ in iter_file_ops[i] if t == 'apply_patch')
        t = len(iter_terminals.get(i, []))
        lines.append(f'| {i} | {c} | {p} | {t} |')
    lines.append(f'| **Celkem** | **{tc}** | **{tp}** | **{tt}** |')

    lines[4] = f'**Souhrn:** {tc} souborů vytvořeno, {tp} editací, {tt} terminálových příkazů'

    output = '\n'.join(lines).replace('\x00', '')

    with open(out_path, 'w') as f:
        f.write(output)

    print(f'Exportováno: {num_iters} iterací, {tc} creates, {tp} patches, {tt} terminals → {out_path}')


if __name__ == '__main__':
    main()
