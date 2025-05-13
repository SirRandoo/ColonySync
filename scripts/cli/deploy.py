from logging import getLogger
from pathlib import Path
from shutil import copyfile
from typing import Final

__all__ = ["deploy_mod"]

_LOGGER: Final[getLogger] = getLogger(__name__)


def deploy_mod(path: Path, dry_run: bool = False):
    """
    Deploys the mod to the specified path. This function ensures that the
    deployment processes are properly handled by verifying the target
    directory's existence, creating missing directories if necessary, and
    executing the deployment logic. It does not allow deployment to the current
    working directory for safety reasons.
    """
    if path == Path.cwd():
        _LOGGER.warning(
            "Cannot deploy to current working directory; aborting...",
        )

        return

    _LOGGER.info(f"Deploying to {path}...")

    if not path.exists():
        try:
            if not dry_run:
                path.mkdir(parents=True, exist_ok=True)
        except OSError as e:
            _LOGGER.exception(
                f"Could not create directory at {path} ; aborting...", exc_info=e
            )

            exit(1)

    _deploy_root_directory(path, dry_run=dry_run)
    _deploy_about_directory(path.joinpath("About"), dry_run=dry_run)


def _deploy_root_directory(root_directory: Path, dry_run: bool = False):
    _LOGGER.info(f"Deploying root directory to {root_directory}...")

    current_directory: Path = Path.cwd()

    _LOGGER.info(f"Deploying README.md to {root_directory}...")

    if not dry_run:
        copyfile(
            current_directory.joinpath("README.md"),
            root_directory.joinpath("README.md"),
        )

    if root_directory.joinpath("LoadFolders.xml").exists():
        _LOGGER.info(f"Deploying LoadFolders.xml to {root_directory}...")

        if not dry_run:
            copyfile(
                current_directory.joinpath("LoadFolders.xml"),
                root_directory.joinpath("LoadFolders.xml"),
            )

    _LOGGER.info(f"Deploying LICENSE to {root_directory}...")

    if not dry_run:
        copyfile(
            current_directory.joinpath("LICENSE"), root_directory.joinpath("LICENSE")
        )


def _deploy_about_directory(about_directory: Path, dry_run: bool = False):
    _LOGGER.info(f"Deploying About directory to {about_directory}...")

    try:
        if not dry_run:
            about_directory.mkdir(parents=True, exist_ok=True)
    except OSError as e:
        _LOGGER.exception(
            f"Could not create About directory at {about_directory} ; aborting...",
            exc_info=e,
        )

        exit(1)

    about_file: Path = about_directory.joinpath("About.xml")
    preview_file: Path = about_directory.joinpath("Preview.png")

    current_directory: Path = Path.cwd()

    _LOGGER.info(f"Deploying About.xml to {about_file}...")

    if not dry_run:
        copyfile(current_directory.joinpath("About/About.xml"), about_file)

    if current_directory.joinpath("About/Preview.png").exists():
        _LOGGER.info(f"Deploying Preview.png to {preview_file}...")

        if not dry_run:
            copyfile(current_directory.joinpath("About/Preview.png"), preview_file)
