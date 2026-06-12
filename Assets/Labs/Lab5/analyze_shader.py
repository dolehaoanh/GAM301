import re
import json

with open('/Users/dolehaoanh/UnityProjects/GAM301/Assets/Labs/Lab5/BulletTrailShader.shadergraph', 'r') as f:
    content = f.read()

# Parse all top-level JSON objects
# Since they are standard JSON blocks starting with '{' and ending with '}' at the start of a line (mostly),
# we can use regex to find matching curly braces or just parse multiple JSON objects using JSONDecoder.
decoder = json.JSONDecoder()
pos = 0
objs = []
content_len = len(content)

while pos < content_len:
    # Skip whitespace
    match = re.match(r'\s*', content[pos:])
    if match:
        pos += match.end()
    if pos >= content_len:
        break
    try:
        obj, count = decoder.raw_decode(content[pos:])
        objs.append(obj)
        pos += count
    except json.JSONDecodeError as e:
        print(f"Error parsing at position {pos}: {e}")
        # Advanced forward by 1 character to try to recover
        pos += 1

print(f"Parsed {len(objs)} objects successfully.")

# Organize objects by ObjectId
db = {}
for obj in objs:
    if isinstance(obj, dict) and 'm_ObjectId' in obj:
        db[obj['m_ObjectId']] = obj

# Find GraphData to list root metadata
graph_data = None
for obj in objs:
    if isinstance(obj, dict) and obj.get('m_Type') == 'UnityEditor.ShaderGraph.GraphData':
        graph_data = obj
        break

print("\n--- Properties ---")
for obj in db.values():
    if obj.get('m_Type') in [
        'UnityEditor.ShaderGraph.Internal.ColorShaderProperty',
        'UnityEditor.ShaderGraph.Internal.Vector1ShaderProperty',
        'UnityEditor.ShaderGraph.Internal.Texture2DShaderProperty'
    ]:
        print(f"Property: '{obj.get('m_Name')}' (ID: {obj.get('m_ObjectId')[-8:]})")
        print(f"  Type: {obj.get('m_Type')}")
        if 'm_ColorMode' in obj:
            print(f"  ColorMode: {'HDR' if obj['m_ColorMode'] == 1 else 'LDR'}")
        if 'm_Value' in obj:
            print(f"  Default Value: {obj['m_Value']}")

print("\n--- Nodes ---")
for obj in db.values():
    if 'Node' in obj.get('m_Type', ''):
        name = obj.get('m_Name', '') or obj.get('m_DisplayName', '')
        print(f"Node: '{name}' | Type: {obj['m_Type']} | ID: {obj['m_ObjectId'][-8:]}")

print("\n--- Blocks ---")
for obj in db.values():
    if obj.get('m_Type') == 'UnityEditor.ShaderGraph.BlockNode':
        print(f"Block: '{obj.get('m_Name')}' | ID: {obj['m_ObjectId'][-8:]}")

print("\n--- Edges/Connections ---")
if graph_data and 'm_Edges' in graph_data:
    for edge in graph_data['m_Edges']:
        out_node_id = edge.get('m_OutputSlot', {}).get('m_Node', {}).get('m_Id', '')
        out_slot = edge.get('m_OutputSlot', {}).get('m_SlotId')
        in_node_id = edge.get('m_InputSlot', {}).get('m_Node', {}).get('m_Id', '')
        in_slot = edge.get('m_InputSlot', {}).get('m_SlotId')
        
        # Get names
        out_node = db.get(out_node_id, {})
        in_node = db.get(in_node_id, {})
        
        out_name = out_node.get('m_Name', '') or out_node.get('m_DisplayName', '') or out_node.get('m_Type', '')
        in_name = in_node.get('m_Name', '') or in_node.get('m_DisplayName', '') or in_node.get('m_Type', '')
        
        print(f"Edge: Node '{out_name}' (ID: {out_node_id[-8:]}) Port {out_slot} -> Node '{in_name}' (ID: {in_node_id[-8:]}) Port {in_slot}")
