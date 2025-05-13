"""
Consolidates duplicate library assemblies into a single "global" assembly cache
in the mod's "Common/Assemblies" folder.
"""

import os
from hashlib import sha256
from pathlib import Path
from typing import Final
from typing import NamedTuple

from utils.click import echo_error
from utils.click import echo_exception
from utils.click import echo_info
from utils.click import echo_warning
from logging import getLogger
from typing import Final

_MOD_ASSEMBLY_PREFIX: Final[str] = "StreamKit"
_MOD_COMMON_ASSEMBLY_PREFIX: Final[str] = "StreamKit.Common"

__all__ = ["consolidate_assemblies"]

_LOGGER: Final[getLogger] = getLogger(__name__)


def consolidate_assemblies(dry_run: bool = False):
    """
    Consolidates assemblies from various versioned directories into a common directory,
    while avoiding duplicates and handling potential hash conflicts appropriately.

    This function scans the current working directory for valid versioned directories,
    retrieves assemblies within those directories, and organizes them into a registry.
    It identifies duplicate assemblies with identical hashes, moves a representative
    assembly to a common directory, and removes the duplicates. In case of conflicting
    hashes for assemblies with the same name, it logs a warning and skips further
    processing of those assemblies.

    Raises:
        SystemExit: If unable to create the common assemblies' directory.

    """
    _LOGGER.info("Consolidating assemblies...")
    assembly_registry: dict[str, list[_Assembly]] = {}

    for current_directory, sub_directories, files in Path.cwd().walk(top_down=True):
        if (
            current_directory.name.startswith(".")
            or current_directory.name.startswith("_")
            or current_directory.name
            in {"node_modules", "obj", "bin", "wwwroot", "src"}
        ):
            sub_directories.clear()
            files.clear()

            continue

        is_version_directory = all(
            [segment.isnumeric() for segment in current_directory.stem.split(".")]
        )

        if not is_version_directory:
            _LOGGER.info(
                f"Skipping {current_directory} as it's not a version directory..."
            )
            continue

        if not current_directory.joinpath("Assemblies").exists():
            _LOGGER.info(
                f"Skipping {current_directory} as it doesn't have an Assemblies directory..."
            )

            continue

        for assembly in current_directory.joinpath("Assemblies").iterdir():
            if assembly.suffix != ".dll" or (
                assembly.stem.startswith(_MOD_ASSEMBLY_PREFIX)
                and not assembly.stem.startswith(_MOD_COMMON_ASSEMBLY_PREFIX)
            ):
                _LOGGER.info(
                    f"Skipping {assembly} as it's either not an assembly or was a mod assembly",
                )
                continue

            with assembly.open("rb") as f:
                assembly_hash = sha256(f.read()).hexdigest()
                assembly_registry.setdefault(assembly.stem, []).append(
                    _Assembly(assembly, assembly_hash)
                )

    common_assemblies_directory: Path = Path.cwd().joinpath("Common", "Assemblies")

    if not common_assemblies_directory.exists() and not dry_run:
        try:
            common_assemblies_directory.mkdir(parents=True, exist_ok=True)
        except OSError as e:
            _LOGGER.exception(
                f"Could not create directory {common_assemblies_directory}; aborting...",
                exc_info=e,
            )

            exit(1)

    for assembly_name, assemblies in assembly_registry.items():
        if len(assemblies) == 1:
            _LOGGER.warning(f"Skipping {assembly_name} as it only has one assembly")

            continue

        unique_hashes = len(set(assembly.hash for assembly in assemblies))

        if unique_hashes > 1:
            _LOGGER.warning(
                f"Found {unique_hashes} assemblies with conflicting hashes for {assembly_name}:",
            )

            for assembly in assemblies:
                _LOGGER.warning(f"  {assembly.file_path} (SHA256: {assembly.hash})")

            _LOGGER.warning(f"Skipping {assembly_name} due to conflicting hashes...")

            continue

        common_assembly = assemblies[0]
        common_assembly_path = common_assemblies_directory.joinpath(
            assembly_name + ".dll"
        )

        _LOGGER.info(f"Moving {common_assembly.file_path} to {common_assembly_path}")

        common_assembly_pdb_path = common_assembly.file_path.with_suffix(".pdb")

        if not dry_run:
            common_assembly.file_path.replace(
                common_assembly_path.joinpath(assembly_name + ".dll")
            )

        if common_assembly_pdb_path.exists() and not dry_run:
            common_assembly_pdb_path.replace(
                common_assembly_path.joinpath(assembly_name + ".pdb")
            )

        _LOGGER.info("Removing duplicate assemblies...")
        for assembly in assemblies[1:]:
            if assembly.file_path.exists():
                _LOGGER.info(f"Removing {assembly.file_path}")

                if dry_run:
                    continue

                assembly_pdb_path = assembly.file_path.with_suffix(".pdb")

                if assembly_pdb_path.exists():
                    assembly_pdb_path.unlink()

                assembly.file_path.unlink()


class _Assembly(NamedTuple):
    file_path: Path
    hash: str
