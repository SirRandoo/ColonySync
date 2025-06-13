from argparse import ArgumentParser
from logging import getLogger
from logging import Logger
from pathlib import Path
from shutil import copy
from shutil import copytree
from typing import Final
from typing import Protocol
from xml.etree import cElementTree as ET

from common import create_default_parser
from common import get_solution_name
from common import init_logging
from common import parse_build_props
from common import parse_packages_props
from common import Version

_LOGGER: Final[Logger] = getLogger(__name__)
_ALWAYS_COPIED_FILE_SUFFIXES: Final[tuple[str, ...]] = (".pdb", ".dll", ".xml")


def get_ignored_files(source: Path, files: list[str]) -> list[str]:
    """Returns a list of files to be ignored when copying from a source directory.

    Args:
        source: The source directory.
        files: The list of files in the source directory.
    Returns:
        The list of files to be ignored.
    """
    container: list[str] = []

    for file in files:
        if "." not in file:
            continue

        *_, suffix = file.split(".")
        suffix = f".{suffix}"

        if suffix not in _ALWAYS_COPIED_FILE_SUFFIXES:
            continue

        container.append(file)

    return container


def parse_author_string(authors: str) -> tuple[str, ...]:
    """Parses a string of authors separated by delimiters into a tuple of individual author names.

    This function takes a string containing author names separated by either semicolons
    or commas, and it returns a tuple of individual author names. It also accounts for
    and removes any leading "and" from the last author's name.

    Args:
        authors (str): A single string of author names separated by semicolons, commas,
            or both.

    Returns:
        tuple[str, ...]: A tuple containing individual author names as strings.
    """
    container: tuple[str, ...] = tuple()

    if authors.index(";"):
        container = tuple(author.strip() for author in authors.split(";"))
    elif authors.index(","):
        container = tuple(author.strip() for author in authors.split(","))

    if container and "and " in container[-1]:
        return tuple(author for author in container[:-1]) + container[-1][len("and ") :]

    return container


def generate_about_file(solution_dir: Path) -> None:
    """Generates an `About.xml` file containing metadata for a RimWorld mod.

    The metadata includes information such as the mod name, author, version, supported game versions,
    and dependencies. The function extracts the relevant data from the `Directory.Build.props` and
    `Directory.Packages.props` files in the given solution directory.

    Args:
        solution_dir:
            The path to the solution directory. This directory should contain the necessary metadata files required
            to generate the About.xml file.
    """
    _LOGGER.info("Generating About.xml file...")
    metadata_element: ET.Element = ET.Element("ModMetaData")
    metadata_element.attrib["Generated"] = "true"

    name_element: ET.Element = ET.SubElement(metadata_element, "name")
    name_element.text = get_solution_name(default_name="MyMod")

    if name_element.text == "MyMod":
        _LOGGER.warning("No solution name found; defaulting to 'MyMod'...")

    author_element: ET.Element = ET.SubElement(metadata_element, "author")
    version_element: ET.Element = ET.SubElement(metadata_element, "modVersion")
    pacakge_id_element: ET.Element = ET.SubElement(metadata_element, "packageId")
    description_element: ET.Element = ET.SubElement(metadata_element, "description")
    supported_versions_element: ET.Element = ET.SubElement(
        metadata_element, "supportedVersions"
    )
    mod_dependencies_element: ET.Element = ET.SubElement(
        metadata_element, "modDependencies"
    )

    _LOGGER.info("Loading properties from Directory.Build.props...")
    with solution_dir.joinpath("Directory.Build.props").open("r") as f:
        build_props = parse_build_props(f)
        f.close()

        authors: tuple[str, ...] = parse_author_string(
            build_props.get("Authors", "Unknown")
        )

        _LOGGER.info("Found %s authors: %s", len(authors), ", ".join(authors))

        if len(authors) > 1:
            author_element.text = authors[0]
            authors_element = ET.SubElement(author_element, "authors")

            for author in authors:
                sub_author_element: ET.Element = ET.SubElement(authors_element, "li")
                sub_author_element.text = author
        else:
            author_element.text = authors[0]

        version_element.text = build_props.get("Version", "0.0.0")

        if version_element.text == "0.0.0":
            _LOGGER.warning("No version found; defaulting to '0.0.0'...")

        pacakge_id_element.text = (
            author_element.text.casefold() + "." + name_element.text.casefold()
        )
        description_element.text = build_props.get(
            "Description", "No description provided."
        )

    _LOGGER.info("Loading properties from Directory.Packages.props...")
    with solution_dir.joinpath("Directory.Packages.props").open("r") as f:
        packages_props = parse_packages_props(f)
        f.close()

        ref_version: Version = packages_props.get("Krafs.Rimworld.Ref")

        _LOGGER.info(
            "Found RimWorld Ref version %s; marking supported version...",
            ref_version,
        )

        current_supported_version_element: ET.Element = ET.SubElement(
            supported_versions_element, "li"
        )
        current_supported_version_element.text = ref_version.to_string(2)

        if "Lib.Harmony" in packages_props:
            _LOGGER.info(
                "Found a dependency on Harmony v%s; marking dependency...",
                packages_props["Lib.Harmony"],
            )

            harmony_dependency_element: ET.Element = ET.SubElement(
                mod_dependencies_element, "li"
            )
            harmony_package_id_element: ET.Element = ET.SubElement(
                harmony_dependency_element, "packageId"
            )
            harmony_display_name_element: ET.Element = ET.SubElement(
                harmony_dependency_element, "displayName"
            )
            harmony_steam_workshop_url_element: ET.Element = ET.SubElement(
                harmony_dependency_element, "steamWorkshopUrl"
            )
            harmony_download_url_element: ET.Element = ET.SubElement(
                harmony_dependency_element, "downloadUrl"
            )

            harmony_package_id_element.text = "brrainz.harmony"
            harmony_display_name_element.text = "Harmony"
            harmony_steam_workshop_url_element.text = (
                "steam://url/CommunityFilePage/2009463077"
            )
            harmony_download_url_element.text = (
                "https://github.com/pardeike/HarmonyRimWorld/releases/latest"
            )
        else:
            metadata_element.remove(mod_dependencies_element)

    with solution_dir.joinpath("About", "About.xml").open("w") as f:
        tree = ET.ElementTree(metadata_element)
        ET.indent(tree, space="  ")
        tree.write(f, xml_declaration=True, encoding="utf-8")


def deploy_about_directory(source_directory: Path, destination_directory: Path) -> None:
    """Deploy 'About' directory contents to a destination directory.

    The function performs several operations within the provided source directory:
    it checks for the presence of specific files ('About.txt', 'Preview.png',
    'PublishedFileId.txt') and copies them to the destination directory if they exist.
    If the 'About.txt' file does not exist, a default version is generated in the
    parent directory of the source. Logging is used to provide information regarding
    the status of operations.

    Args:
        source_directory: Source directory containing the files to deploy.
        destination_directory: Destination directory where files are copied.

    """
    about_file = source_directory.joinpath("About.txt")

    if not about_file.exists():
        _LOGGER.warning(
            "About.xml file not found; generating a default About.txt file..."
        )

        generate_about_file(source_directory.parent)

    _LOGGER.info("Copying About.xml to destination directory %s", destination_directory)
    copy(about_file, destination_directory)
    _LOGGER.info("Done!")

    preview_file = source_directory.joinpath("Preview.png")

    if preview_file.exists():
        _LOGGER.info(
            "Copying Preview.png to destination directory %s", destination_directory
        )
        copy(preview_file, destination_directory)
        _LOGGER.info("Done!")

    workshop_id_file = source_directory.joinpath("PublishedFileId.txt")

    if workshop_id_file.exists():
        _LOGGER.info(
            "Copying PublishedFileId.txt to destination directory %s",
            destination_directory,
        )
        copy(workshop_id_file, destination_directory)
        _LOGGER.info("Done!")


def deploy_top_level_files(source_directory: Path, destination_directory: Path) -> None:
    """Copies top-level files from the source directory to the destination directory.

    Args:
        source_directory:
            Path to the source directory containing the files to be copied.
        destination_directory:
            Path to the destination directory where files will be copied.
    """
    license_file: Path

    if source_directory.joinpath("LICENSE").exists():
        license_file = source_directory.joinpath("LICENSE")
    elif source_directory.joinpath("License.md").exists():
        license_file = source_directory.joinpath("License.md")
    else:
        license_file = source_directory.joinpath("LICENSE.txt")

    if license_file.exists():
        _LOGGER.info(
            "Copying LICENSE file to destination directory %s", destination_directory
        )
        copy(license_file, destination_directory)
        _LOGGER.info("Done!")

    readme_file: Path = source_directory.joinpath("README.md")

    if readme_file.exists():
        _LOGGER.info(
            "Copying README file to destination directory %s", destination_directory
        )
        copy(readme_file, destination_directory)
        _LOGGER.info("Done!")


def deploy_releases_directory(
    source_directory: Path, destination_directory: Path
) -> None:
    """Deploys the 'Releases' directory to the destination directory.

    Args:
        source_directory:
            The source directory containing the 'Releases' directory.
        destination_directory:
            The destination directory where the 'Releases' directory will be deployed.
    """
    _LOGGER.info("Deploying compiled assemblies...")

    if not source_directory.exists():
        _LOGGER.error("Releases directory doesn't exist; aborting...")

        return

    if not destination_directory.exists():
        _LOGGER.info(
            "No 'Releases' directory in the destination directory; " "creating..."
        )
        destination_directory.mkdir()

    for item in source_directory.iterdir():
        destination_item: Path = destination_directory.joinpath(item.name)

        if destination_item.exists():
            _LOGGER.warning(
                "Removing existing destination directory %s...", destination_item
            )

            destination_item.rmdir()

        _LOGGER.info("Copying %s to %s...", item, destination_item)
        copytree(
            item,
            destination_item,
            dirs_exist_ok=True,
            ignore=get_ignored_files,
        )


def deploy_project_directory(
    source_directory: Path, destination_directory: Path
) -> None:
    _LOGGER.info("Deploying About directory...")
    deploy_about_directory(
        source_directory.joinpath("About"), destination_directory.joinpath("About")
    )
    _LOGGER.info("Done!")

    _LOGGER.info("Deploying top-level files...")
    deploy_top_level_files(source_directory, destination_directory)
    _LOGGER.info("Done!")

    _LOGGER.info("Deploying release files...")
    deploy_releases_directory(
        source_directory.joinpath("Releases"),
        destination_directory.joinpath("Releases"),
    )
    _LOGGER.info("Done!")


class NamespaceProtocol(Protocol):
    dry_run: bool
    level: str
    destination: Path
    source: Path | None


if __name__ == "__main__":
    parser: ArgumentParser = create_default_parser()
    parser.add_argument(
        "destination",
        type=Path,
        nargs="?",
        help="Destination directory path.",
        required=True,
    )
    parser.add_argument("--source", type=Path, nargs="?", help="Source directory path.")

    # noinspection PyTypeChecker
    parsed: NamespaceProtocol = parser.parse_args()

    # noinspection PyTypeChecker
    init_logging(parsed.level)
