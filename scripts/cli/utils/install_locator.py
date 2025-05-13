from pathlib import Path
from platform import system
from logging import getLogger
from typing import Final
from cli.vdf_parser import parse_vdf

_LOGGER: Final[getLogger] = getLogger(__name__)


def _locate_steam_install_vdf(vdf_path: Path) -> Path | None:
    """
    Locate the Steam installation path from the provided VDF file.

    Args:
        vdf_path: The file path to the VDF file to be opened and read.

    Returns:
        Path or None: The located Steam installation path if found, otherwise None.
    """
    with vdf_path.open() as f:
        contents = parse_vdf(f.read())
        print(contents["libraryfolders"]["0"])

        for key, value in contents.get("libraryfolders", {}).items():
            # noinspection PyTypeChecker
            raw_path: str = None if value is None else value.get("path", None)

            if raw_path is None:
                continue

            path = Path(raw_path)

            if path.joinpath(
                "steamapps", "common", "RimWorld", "Data", "Core", "About", "About.xml"
            ).exists():
                return path


def locate_game_install() -> Path | None:
    match system().casefold():
        case "windows":
            return _locate_windows_game_install()
        case "linux":
            return _locate_linux_game_install()
        case "darwin":
            return _locate_mac_game_install()
        case _:
            _LOGGER.warning("Unsupported platform detected: %s", system())


def _locate_windows_game_install() -> Path | None:
    known_steam_install_directory: Path = Path("C:/Program Files (x86)/Steam")

    if not known_steam_install_directory.exists():
        _LOGGER.warning(
            "Non-standard Steam install detected; create a file called '.steam' in the '.run' directory with the path to the Steam installation as its contents"
        )

        return None

    steam_config_path: Path = known_steam_install_directory.joinpath(
        "config", "libraryfolders.vdf"
    )

    if not steam_config_path.exists():
        _LOGGER.warning(
            "Standard Steam install detected, but no libraryfolders.vdf found"
        )
        _LOGGER.warning(
            "Create a file called '.steam' in the '.run' directory with the path to the Steam installation as its contents"
        )

        return None

    return _locate_steam_install_vdf(steam_config_path)


def _locate_linux_game_install() -> Path | None:
    home_directory: Path = Path.home()
    steam_directory: Path = home_directory.joinpath(".steam", "steam")

    if not steam_directory.exists():
        _LOGGER.warning(
            "Non-standard Steam install detected; create a file called '.steam' in the '.run' directory with the path to the Steam installation as its contents"
        )

        return None

    library_folders_file: Path = steam_directory.joinpath(
        "config", "libraryfolders.vdf"
    )

    if library_folders_file.exists():
        _LOGGER.warning(
            "Standard Steam install detected, but no libraryfolders.vdf found"
        )
        _LOGGER.warning(
            "Create a file called '.steam' in the '.run' directory with the path to the Steam installation as its contents"
        )

        return None

    return _locate_steam_install_vdf(library_folders_file)


def _locate_mac_game_install() -> Path | None:
    home_directory: Path = Path.home()
    steam_directory: Path = home_directory.joinpath(
        "Library", "Application Support", "Steam"
    )

    if not steam_directory.exists():
        _LOGGER.warning(
            "Non-standard Steam install detected; create a file called '.steam' in the '.run' directory with the path to the Steam installation as its contents"
        )

        return None

    library_folders_file: Path = steam_directory.joinpath(
        "config", "libraryfolders.vdf"
    )

    if not library_folders_file.exists():
        _LOGGER.warning(
            "Standard Steam install detected, but no libraryfolders.vdf found"
        )
        _LOGGER.warning(
            "Create a file called '.steam' in the '.run' directory with the path to the Steam installation as its contents"
        )

    return _locate_steam_install_vdf(library_folders_file)
