from argparse import ArgumentParser
from dataclasses import dataclass
from dataclasses import field
from dataclasses import replace
from io import TextIOWrapper
from logging import getLogger
from logging import Logger
from pathlib import Path
from typing import Final
from typing import Protocol
from xml.etree import cElementTree as ET

from common import create_default_parser
from common import get_default_save_data_path
from common import init_logging
from common import load_metadata
from common import ModMetaData

_LOGGER: Final[Logger] = getLogger(__name__)


class NamespaceProtocol(Protocol):
    mod_id: list[str]
    dry_run: bool
    level: str
    save_data: Path | None


@dataclass(frozen=True, slots=True)
class ModsConfig:
    """Represents the game's mod load configuration.

    Attributes:
        version: The version of the game that wrote this config.
        active_mods: The list of active mods.
        known_expansions: The list of known expansions.
    """

    version: str
    active_mods: tuple[str, ...] = field(default_factory=tuple)
    known_expansions: tuple[str, ...] = field(default_factory=tuple)


def load_mods_config(file: TextIOWrapper) -> ModsConfig:
    """Loads mods config from an XML file."""
    tree = ET.ElementTree(file=file)
    root: ET.Element = tree.getroot()

    mods_config_version: str | None = None
    mods_config_active_mods: tuple[str, ...] = tuple()
    mods_config_known_expansions: tuple[str, ...] = tuple()

    for element in root:
        match element.tag:
            case "version":
                mods_config_version = element.text
            case "activeMods":
                mods_config_active_mods = tuple(child.text for child in element)
            case "knownExpansions":
                mods_config_known_expansions = tuple(child.text for child in element)

    return ModsConfig(
        mods_config_version, mods_config_active_mods, mods_config_known_expansions
    )


def dump_mods_config(obj: ModsConfig, file: TextIOWrapper) -> None:
    """Saves mods config to an XML file."""
    root: ET.Element = ET.Element("ModsConfigData")

    version_node = ET.SubElement(root, "version")
    version_node.text = obj.version

    active_mods_node = ET.SubElement(root, "activeMods")

    for active_mod in obj.active_mods:
        active_mod_node = ET.SubElement(active_mods_node, "li")
        active_mod_node.text = active_mod

    known_expansions_node = ET.SubElement(root, "knownExpansions")

    for known_expansion in obj.known_expansions:
        known_expansion_node = ET.SubElement(known_expansions_node, "li")
        known_expansion_node.text = known_expansion

    tree = ET.ElementTree(root)
    tree.write(file, encoding="utf-8", xml_declaration=True)


if __name__ == "__main__":

    parser: ArgumentParser = create_default_parser()
    parser.add_argument(
        "--mod_id", type=str, nargs="*", help="The id of the mod that should be active."
    )
    parser.add_argument(
        "--save-data",
        type=Path,
        nargs="?",
        help="The path to the game's save data directory.",
    )

    # noinspection PyTypeChecker
    parsed: NamespaceProtocol = parser.parse_args()

    # noinspection PyTypeChecker
    init_logging(parsed.level)

    if not parsed.mod_id:
        _LOGGER.warning("No mod id was passed; loading mod id from metadata file...")

        with Path("About/About.xml").open() as in_file:
            metadata: ModMetaData = load_metadata(in_file)
            parsed.mod_id = [metadata.id]

        _LOGGER.info("Loaded mod id (%s) from metadata file", metadata.id)

    if not parsed.save_data:
        _LOGGER.warning("No save data path was passed; using default path...")

        parsed.save_data = get_default_save_data_path()

        _LOGGER.info("Defaulted to global save data path %s", parsed.save_data)

    _LOGGER.info("Ensuring the following mods are active: %s", ", ".join(parsed.mod_id))

    mods_config_path: Path = parsed.save_data.joinpath("Config", "ModsConfig.xml")
    with mods_config_path.open("r") as in_file:
        config: ModsConfig = load_mods_config(in_file)
        in_file.close()

        for mod_id in parsed.mod_id:
            _LOGGER.info("Is mod '%s' active?", mod_id in config.active_mods)

            if mod_id not in config.active_mods:
                config = replace(config, active_mods=config.active_mods + (mod_id,))

        if parsed.dry_run:
            _LOGGER.info("Dry run; not saving changes.")

            exit(0)

        with mods_config_path.open("w") as out_file:
            dump_mods_config(config, out_file)
