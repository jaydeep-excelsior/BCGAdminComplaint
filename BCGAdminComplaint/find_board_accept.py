import json
import sys

transcript_path = r"C:\Users\info\.gemini\antigravity-ide\brain\983283c7-63d5-4b39-9c14-819069f2dac7\.system_generated\logs\transcript_full.jsonl"
found_board_accept = False

with open(transcript_path, 'r', encoding='utf-8') as f:
    for line in f:
        try:
            data = json.loads(line)
            content = data.get("content", "")
            if 'id="boardAccept"' in content or "id='boardAccept'" in content:
                print(f"Found in step {data.get('step_index')}:")
                # find the index of boardAccept and print context around it
                idx = content.find('id="boardAccept"')
                if idx != -1:
                    print(content[max(0, idx-500) : min(len(content), idx+2000)])
                found_board_accept = True
        except Exception as e:
            pass

if not found_board_accept:
    print("Could not find boardAccept in the full transcript.")
