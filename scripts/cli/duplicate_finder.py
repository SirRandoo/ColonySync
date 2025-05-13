from logging import getLogger
from pathlib import Path
from typing import Final
from typing import NamedTuple
from xml.etree import ElementTree as ET

__all__ = ["find_duplicates"]

_LOGGER: Final[getLogger] = getLogger(__name__)


def find_duplicates(path: Path):
    """
    Scans the specified directory (and its subdirectories) for duplicate translations in XML
    files. XML files should consist of a root element with child elements where the text content
    of the children is checked for duplicates. If duplicates are found, they will be logged and
    counted. Assumes a structured directory path for language data and falls back to defaults
    if no path is provided.

    Args:
        path:
            The directory path to scan for duplicate translations. If the given directory is
            the current working directory, a default sub-path is appended.
    """
    if path == Path.cwd():
        _LOGGER.warning("No language root directory was provided, assuming defaults...")

        path = Path.cwd().joinpath("Common", "Languages", "English", "Keyed")

    _LOGGER.info(f"Scanning for duplicates in directory {path}")

    translations: dict[str, set[_Translation]] = {}

    for current_directory, sub_directories, files in path.walk():
        for file in files:
            if not file.endswith(".xml"):
                continue

            with current_directory.joinpath(file).open("r") as f:
                tree = ET.ElementTree(file=f)
                root = tree.getroot()

                for child in root:
                    translations.setdefault(child.text, set()).add(
                        _Translation(current_directory.joinpath(file), child.tag)
                    )

    duplicates_found: int = 0
    for text, keys in translations.items():
        if len(keys) > 1:
            _LOGGER.warning(
                f"  The translation '{text}' was duplicated among {len(keys)} keys:",
            )
            duplicates_found += len(keys) - 1

            for key in keys:
                _LOGGER.warning(f"    - {key.key} ({key.file_path.relative_to(path)})")

    _LOGGER.warning(f"Found a total of {duplicates_found} duplicate translations")


class _Translation(NamedTuple):
    file_path: Path
    key: str
