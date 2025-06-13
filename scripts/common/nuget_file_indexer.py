from dataclasses import dataclass
from dataclasses import field
from logging import getLogger
from logging import Logger
from os import getenv
from pathlib import Path
from typing import Final

from .nuspec_parser import load_nuspec
from .nuspec_parser import NuSpec
from .version import Version

__all__ = ["index_package", "NuGetPackageIndex"]

_LOGGER: Final[Logger] = getLogger(__name__)
_DEFAULT_PACAKGE_DIRECTORY: Final[Path] = Path.home().joinpath(".nuget", "packages")
_PACKAGE_DIRECTORY_OVERRIDE: Final[str | None] = getenv("PACKAGE_DIRECTORY_OVERRIDE")
_NUGET_PACKAGE_DIRECTORY: Final[Path] = Path(
    _PACKAGE_DIRECTORY_OVERRIDE or _DEFAULT_PACAKGE_DIRECTORY
)


@dataclass(frozen=True, slots=True)
class NuGetPackageIndex:
    nuspec: NuSpec
    framework_files: dict[str, tuple[Path, ...]] = field(default_factory=dict)
    runtime_files: dict[str, tuple[Path, ...]] = field(default_factory=dict)


def index_package(package_id: str, version: Version) -> NuGetPackageIndex:
    """Indexes a specific version of a NuGet package.

    This function accesses the filesystem to parse the structure and retrieve information
    from a specific NuGet package version. It requires a valid package ID and version to
    operate. If the expected directories or files do not exist as part of the package,
    specific exceptions will be raised.

    Args:
        package_id: The unique identifier of the NuGet package.
        version: The version of the NuGet package to index.
    Returns:
        NuGetPackageIndex:
            An object representing the indexed package, containing metadata and file
            structure information.
    Raises:
        FileNotFoundError: Raised when the package's `.nuspec` file is missing.
        NotADirectoryError:
            Raised when either the package directory or the specific version
            directory does not exist.
    """
    package_directory: Path = _NUGET_PACKAGE_DIRECTORY.joinpath(package_id)

    if not package_directory.exists():
        _LOGGER.warning("No such package: %s", package_id)

        raise NotADirectoryError(str(package_directory))

    package_version_directory: Path = package_directory.joinpath(str(version))

    if not package_version_directory.exists():
        _LOGGER.warning("No such package version: %s", str(version))

        raise NotADirectoryError(str(package_version_directory))

    nuspec_file: Path = package_version_directory.joinpath(f"{package_id}.nuspec")

    if not nuspec_file.exists():
        _LOGGER.warning("No such nuspec file: %s", str(nuspec_file))

        raise FileNotFoundError(str(nuspec_file))

    with nuspec_file.open("r") as in_file:
        nuspec: NuSpec = load_nuspec(in_file)
        in_file.close()

    instance: NuGetPackageIndex = NuGetPackageIndex(nuspec)

    for directory in package_version_directory.joinpath("lib").iterdir():
        for framework_directory in directory.iterdir():
            if framework_directory.stem not in instance.framework_files:
                instance.framework_files[framework_directory.stem] = tuple()

            framework_files: tuple[Path, ...] = instance.framework_files[
                framework_directory.stem
            ]

            for current_directory, directories, files in framework_directory.walk():
                framework_files += (current_directory.joinpath(file) for file in files)

            instance.framework_files[framework_directory.stem] = framework_files

    for directory in package_version_directory.joinpath("runtimes").iterdir():
        for runtime_directory in directory.iterdir():
            if runtime_directory.stem not in instance.runtime_files:
                instance.runtime_files[runtime_directory.stem] = tuple()

            runtime_files: tuple[Path, ...] = instance.runtime_files[
                runtime_directory.stem
            ]

            for current_directory, directories, files in runtime_directory.walk():
                runtime_files += (current_directory.joinpath(file) for file in files)

            instance.runtime_files[runtime_directory.stem] = runtime_files

    return instance
