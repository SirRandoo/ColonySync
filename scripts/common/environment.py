import platform
from logging import getLogger
from logging import Logger
from pathlib import Path
from typing import Final

__all__ = ["get_solution_name", "get_default_save_data_path"]


_LOGGER: Final[Logger] = getLogger(__name__)


def get_solution_name(*, default_name: str | None = None) -> str:
    """Scans the current working directory for the name of the solution.

    Returns:
        The name of the solution.
    """
    _LOGGER.info("Scanning for solution name...")

    for entry in Path.cwd().iterdir():
        if entry.suffix in {".sln"}:
            slnx_file = entry.with_suffix(".slnx")

            if slnx_file.exists():
                _LOGGER.info("Solution name is '%s'", slnx_file.stem)

                return slnx_file.stem
            else:
                _LOGGER.info("Solution name is '%s'", entry.stem)

                return entry.stem

    _LOGGER.warning(
        "No solution file could be found; execute the script in the solution directory"
    )
    return default_name or "rw-mod"


def get_default_save_data_path() -> Path:
    """Returns the default path to the game's save data directory.

    Raises:
        UnsupportedPlatformError:
            Raised when the script runs on an unsupported platform.
    """
    match platform.system().casefold():
        case "windows":
            return Path.home().joinpath(
                "AppData", "LocalLow", "Ludeon Studios", "RimWorld by Ludeon Studios"
            )
        case "darwin":
            return Path.home().joinpath(
                "Library",
                "Application Support",
                "Ludeon Studios",
                "RimWorld by Ludeon Studios",
            )
        case "linux":
            return Path.home().joinpath(
                ".config", "unity3d", "Ludeon Studios", "RimWorld by Ludeon Studios"
            )
        case _:
            raise UnsupportedPlatformError()


class UnsupportedPlatformError(Exception):
    """Raised when an operation determines it's unsupported on the current platform."""
