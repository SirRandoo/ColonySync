from collections import deque
from typing import NamedTuple

type VdfColor = str
type VdfWString = str
type VdfValue = int | None | float | bool | str | VdfColor | VdfWString | dict[
    str, VdfValue
]
type VdfDict = dict[str, VdfValue]


class _VdfNode(NamedTuple):
    key_name: str
    value: VdfValue


def _parse_value(value: str) -> VdfValue:
    if value.startswith('"'):
        return value[1:-1]

    if value in ("0", "1"):
        return value == "1"
    if value.isdigit():
        return int(value)
    if value.isdecimal():
        return float(value)


def parse_vdf(vdf_string: str) -> VdfDict:
    """
    Parses a VDF (Valve Data Format) string and converts it into a nested dictionary structure.

    The function processes a VDF-formatted string, recognizing its hierarchical structure, and
    maps its keys to corresponding values or nested dictionaries. It uses a stack-based approach
    to handle the nested blocks defined by curly braces in the VDF structure.

    Args:
        vdf_string (str): A string containing VDF data to be parsed.

    Returns:
        VdfDict: A dictionary representing the hierarchical structure of the parsed VDF data.
    """
    root: VdfDict = {}
    stack: deque[_VdfNode] = deque()

    def get_node_container(node: _VdfNode | None = None) -> VdfDict:
        if node is None:
            if len(stack) > 0:
                return stack[-1].value
            else:
                return root

        if stack.count(node) == 0:
            return root
        if stack.count(node) == 1:
            return root

        return stack[stack.index(node) - 1].value

    last_key: str | None = None

    for line, text in enumerate(vdf_string.split("\n"), 1):
        text = text.lstrip()

        if text.startswith("/"):
            continue

        segments = text.split()

        if len(segments) == 2 and all(
            [s.startswith('"') and s.endswith('"') for s in segments]
        ):
            get_node_container()[segments[0][1:-1]] = _parse_value(segments[1])

            continue

        if text.startswith('"'):
            last_key = text[1:-1]
            _node = _VdfNode(text[1:-1], None)
            get_node_container()[text[1:-1]] = None
        elif text.startswith("{"):
            _node = _VdfNode(last_key, {})
            stack.append(_node)
        elif text.startswith("}"):
            _node = stack.pop()
            get_node_container()[_node.key_name] = _node.value

    return root
