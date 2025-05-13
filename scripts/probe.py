from io import UnsupportedOperation
from pathlib import Path
from platform import system
from cli.vdf_parser import parse_vdf

try:
    from os import listdrives
except ImportError:
    pass  # We're not on windows

__all__ = ["locate_steam_install"]


def locate_steam_install() -> Path | None:
    """
    This function locates the Steam installation directory on the user's machine based on the
    operating system. Currently, it only supports locating the Steam installation directory on
    Windows platforms.

    Returns:
        Path | None: The path to the Steam installation directory if found, otherwise None.

    Raises:
        UnsupportedOperation: If the operating system is not supported.
    """

    platform_str: str = system().casefold()

    match platform_str:
        case "windows":
            return _locate_steam_install_windows()
        case "linux":
            return _scan_steam_install_linux()
        case _:
            raise UnsupportedOperation(f"{platform_str} is not a supported platform")


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

            print(key, type(key), raw_path)

            if raw_path is None:
                continue

            path = Path(raw_path)

            if path.joinpath(
                "steamapps", "common", "RimWorld", "Data", "Core", "About", "About.xml"
            ).exists():
                return path


def _locate_steam_install_windows() -> Path | None:
    """
    Locate the Steam installation folder on a Windows system.

    It attempts to locate the Steam installation by checking common installation paths:
    1. Checks if the `libraryfolders.vdf` file exists in the default `Program Files (x86)` path. If found, uses it to locate the installation.
    2. Searches for the `RimWorld` game directory in the default `Steam` path within `Program Files (x86)`.
    3. Searches for the `RimWorld` game directory in the Steam library on the D: drive.
    4. Calls a function to scan the system for the Steam installation if none of the above succeed.

    Returns:
        Path | None: The path to the Steam installation directory, or None if not found.
    """
    program_files_x86_vdf: Path = Path(
        "C:\\Program Files (x86)\\Steam\\steamapps\\config\\libraryfolders.vdf"
    )

    if program_files_x86_vdf.exists():
        result: Path | None = _locate_steam_install_vdf(program_files_x86_vdf)

        if result:
            return result

    program_files_x86: Path = Path("C:\\Program Files (x86)\\Steam\\steamapps")

    if program_files_x86.joinpath(
        "common\\RimWorld\\Data\\Core\\About\\About.xml"
    ).exists():
        return program_files_x86

    d_drive: Path = Path("D:\\SteamLibrary\\steamapps")

    if d_drive.joinpath("common\\RimWorld\\Data\\Core\\About\\About.xml").exists():
        return d_drive

    return _scan_steam_install_windows()


def _scan_steam_install_windows() -> Path | None:
    """
    Search for the Steam installation directory on a Windows system.

    This function scans all available drives on the system to locate the directory
    containing the "steam.exe" executable. It traverses directories up to a depth
    of 3 levels starting from the drive root and stops searching deeper if the
    depth limit is reached. If the Steam installation is found, the path to the
    directory is returned. If not found, None is returned.

    Returns:
        Path | None: The path to the directory containing "steam.exe" if found,
        otherwise None.
    """
    for drive in listdrives():
        root_path: Path = Path(drive)

        for path, directories, files in root_path.walk(top_down=True):
            # We'll only index 3 nodes deep, including the drive.
            if len(path.parts) > 3:
                # noinspection PyUnusedLocal
                directories = []

                continue

            for directory in directories:
                directory_path: Path = path.joinpath(directory)

                if directory_path.joinpath("steam.exe"):
                    return directory_path


def _scan_steam_install_linux() -> Path | None:
    """
    Scans the Steam installation directory on a Linux system and identifies the Steam
    installation path.

    This function checks the user's home directory for the default Steam directory
    (".steam/steam"). It verifies the existence of the "libraryfolders.vdf" configuration
    file within the directory and processes it to locate the Steam installation path.

    Returns:
        Path | None: The path to the Steam installation directory if it exists, or None
        if the directory or configuration file is not found.
    """
    home_directory: Path = Path.home()
    steam_directory: Path = home_directory.joinpath(".steam", "steam")
    library_folders_file: Path = steam_directory.joinpath(
        "config", "libraryfolders.vdf"
    )

    if library_folders_file.exists():
        return _locate_steam_install_vdf(library_folders_file)
