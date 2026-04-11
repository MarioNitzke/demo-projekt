#!/usr/bin/env python3
"""Export Claude Code konverzace z JSONL transcriptu do čitelného Markdown."""
import json, sys, os, glob


def format_tool_use(content_block):
    """Format tool_use block."""
    name = content_block.get('name', '')
    inp = content_block.get('input', {})

    if name in ('Read', 'read_file'):
        path = inp.get('file_path', inp.get('path', ''))
        return f'> **{name}**: {path}'
    elif name in ('Write', 'write_file', 'create_file'):
        path = inp.get('file_path', inp.get('path', ''))
        content = inp.get('content', '')
        lines = content.count('\n') + 1 if content else 0
        return f'> **{name}**: {path} ({lines} řádků)'
    elif name in ('Edit', 'edit_file'):
        path = inp.get('file_path', inp.get('path', ''))
        return f'> **{name}**: {path}'
    elif name == 'Bash':
        cmd = inp.get('command', '')
        desc = inp.get('description', '')
        if desc:
            return f'> **Bash**: {desc}\n> ```\n> {cmd[:500]}\n> ```'
        return f'> **Bash**:\n> ```\n> {cmd[:500]}\n> ```'
    elif name == 'Glob':
        return f'> **Glob**: {inp.get("pattern", "")}'
    elif name == 'Grep':
        return f'> **Grep**: {inp.get("pattern", "")} in {inp.get("path", "")}'
    elif name == 'Agent':
        return f'> **Agent**: {inp.get("description", "")}'
    elif name.startswith('mcp__'):
        return f'> **MCP {name}**: {json.dumps(inp, ensure_ascii=False)[:200]}'
    else:
        return f'> **{name}**: {json.dumps(inp, ensure_ascii=False)[:300]}'


def format_tool_result(content_block):
    """Format tool_result block."""
    result = content_block.get('content', '')
    if isinstance(result, list):
        texts = [r.get('text', '') for r in result if isinstance(r, dict)]
        result = '\n'.join(texts)
    if isinstance(result, str) and len(result) > 500:
        return f'> Výsledek: {result[:500]}...'
    elif result:
        return f'> Výsledek: {result}'
    return None


def process_jsonl(jsonl_path):
    """Process JSONL file into conversation entries."""
    entries = []

    with open(jsonl_path, 'r') as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            try:
                obj = json.loads(line)
            except:
                continue

            # Skip progress/hook events
            if obj.get('type') == 'progress':
                continue

            timestamp = obj.get('timestamp', '')
            message = obj.get('message', {})
            role = message.get('role', '')
            content = message.get('content', '')

            if role == 'user':
                if isinstance(content, str):
                    # Skip system reminders
                    import re
                    clean = re.sub(r'<system-reminder>.*?</system-reminder>', '', content, flags=re.DOTALL).strip()
                    if clean:
                        entries.append(('user', clean))
                elif isinstance(content, list):
                    texts = []
                    has_tool_result = False
                    for block in content:
                        if isinstance(block, dict):
                            if block.get('type') == 'text':
                                text = block.get('text', '').strip()
                                import re
                                text = re.sub(r'<system-reminder>.*?</system-reminder>', '', text, flags=re.DOTALL).strip()
                                if text:
                                    texts.append(text)
                            elif block.get('type') == 'tool_result':
                                has_tool_result = True
                                # Skip tool results — they're responses to tool_use, not user messages
                        elif isinstance(block, str):
                            texts.append(block)
                    if texts and not has_tool_result:
                        entries.append(('user', '\n'.join(texts)))

            elif role == 'assistant':
                if isinstance(content, list):
                    for block in content:
                        if not isinstance(block, dict):
                            continue
                        btype = block.get('type', '')
                        if btype == 'text':
                            text = block.get('text', '').strip()
                            if text:
                                entries.append(('assistant_text', text))
                        elif btype == 'tool_use':
                            entries.append(('tool_use', format_tool_use(block)))
                        elif btype == 'thinking':
                            # Skip thinking blocks - too verbose
                            pass
                elif isinstance(content, str) and content.strip():
                    entries.append(('assistant_text', content.strip()))

    return entries


def to_markdown(entries, session_id=''):
    """Convert entries to markdown."""
    lines = [
        f'# Claude Code Session',
        f'',
        f'Session: `{session_id}`',
        f'',
        f'---',
        f''
    ]

    prev_role = None
    for etype, content in entries:
        if etype == 'user':
            # Strip system reminders
            clean = content
            for tag in ['<system-reminder>', '</system-reminder>']:
                if tag in clean:
                    # Remove system reminder blocks
                    import re
                    clean = re.sub(r'<system-reminder>.*?</system-reminder>', '', clean, flags=re.DOTALL).strip()

            if not clean:
                continue

            if prev_role != 'user':
                lines.append('## Uživatel')
                lines.append('')
            lines.append(clean)
            lines.append('')
            prev_role = 'user'

        elif etype == 'assistant_text':
            if prev_role != 'assistant':
                lines.append('## Claude Code')
                lines.append('')
            lines.append(content)
            lines.append('')
            prev_role = 'assistant'

        elif etype == 'tool_use':
            if prev_role != 'assistant':
                lines.append('## Claude Code')
                lines.append('')
            lines.append(content)
            lines.append('')
            prev_role = 'assistant'

    return '\n'.join(lines)


def main():
    if len(sys.argv) >= 2:
        jsonl_path = sys.argv[1]
    else:
        # Find latest JSONL in default location
        pattern = os.path.expanduser('~/.claude/projects/-Users-marioboss-diplomova-prace/*.jsonl')
        files = glob.glob(pattern)
        if not files:
            print('No JSONL files found')
            return
        jsonl_path = max(files, key=os.path.getmtime)
        print(f'Using latest: {jsonl_path}')

    out_path = sys.argv[2] if len(sys.argv) >= 3 else 'konverzace.md'

    session_id = os.path.splitext(os.path.basename(jsonl_path))[0]
    entries = process_jsonl(jsonl_path)
    md = to_markdown(entries, session_id)

    with open(out_path, 'w') as f:
        f.write(md)

    user_count = len([e for e in entries if e[0] == 'user'])
    tool_count = len([e for e in entries if e[0] == 'tool_use'])
    text_count = len([e for e in entries if e[0] == 'assistant_text'])
    print(f'Exportováno: {user_count} user, {text_count} odpovědí, {tool_count} tool calls → {out_path}')


if __name__ == '__main__':
    main()
