from dataclasses import dataclass
from dataclasses import field
from io import TextIOWrapper
from xml.etree import cElementTree as ET

from .version import Version

__all__ = ["ModDependency", "ModMetaData", "load_metadata"]


@dataclass(frozen=True, slots=True)
class ModDependency:
    """Represents a dependency a mod may have.

    Attributes:
        id:
            The package id of the mod. This is typically a period-separated string.
        name:
            The human-readable name of the mod.
        download_url:
            The link to a "releases" page for the mod. This is typically a GitHub
            repository's "releases" section.
        steam_workshop_url:
            The Steam link to the mod's workshop page.
    """

    id: str
    name: str
    download_url: str
    steam_workshop_url: str


@dataclass(frozen=True, slots=True)
class ModMetaData:
    """Represents the mod's metadata file, typically called "About.xml".

    Attributes:
        id:
            The package id of the mod. This is typically a period-separated string.
        name:
            The human-readable name of the mod.
        version:
            The current version of the mod.
        description:
            A brief description of the mod.
        url:
            A url to the mod's website. This is typically a GitHub repository url.
        supported_versions:
            The list of game versions the mod supports.
        authors:
            The names of the people who've written the mod or otherwise
            contributed to it.
        dependencies:
            A collection of dependencies the mod may have.
    """

    id: str
    name: str
    version: Version = field(default_factory=lambda: Version(0, 1, 0))
    url: str | None = field(default=None)
    description: str | None = field(default=None)
    supported_versions: tuple[Version, ...] = field(default_factory=tuple)
    authors: tuple[str, ...] = field(default_factory=tuple)
    dependencies: tuple[ModDependency, ...] = field(default_factory=tuple)


def _parse_dependency(element: ET.Element) -> ModDependency:
    dependency_id: str | None = None
    dependency_name: str | None = None
    dependency_download_url: str | None = None
    dependency_steam_workshop_url: str | None = None

    for child in element:
        match child.tag:
            case "id":
                dependency_id = child.text
            case "name":
                dependency_name = child.text
            case "downloadUrl":
                dependency_download_url = child.text
            case "steamWorkshopUrl":
                dependency_steam_workshop_url = child.text

    return ModDependency(
        dependency_id,
        dependency_name,
        dependency_download_url,
        dependency_steam_workshop_url,
    )


def load_metadata(file: TextIOWrapper) -> ModMetaData:
    """Loads metadata from an "About.xml" file."""
    tree = ET.ElementTree(file=file)
    root: ET.Element = tree.getroot()

    mod_id: str | None = None
    mod_name: str | None = None
    mod_version: Version | None = Version(0, 1, 0)
    mod_description: str | None = None
    mod_url: str | None = None
    mod_supported_versions: tuple[Version, ...] = tuple()
    mod_authors: tuple[str, ...] = tuple()
    mod_dependencies: tuple[ModDependency, ...] = tuple()

    for element in root:
        match element.tag:
            case "name":
                mod_name = element.text
            case "packageId":
                mod_id = element.text
            case "author":
                mod_authors = tuple([element.text] + list(mod_authors))
            case "description":
                mod_description = element.text
            case "modVersion":
                mod_version = Version.parse(element.text)
            case "supportedVersions":
                mod_supported_versions = tuple(
                    Version.parse(child.text) for child in element
                )
            case "modDependencies":
                mod_dependencies = tuple(_parse_dependency(child) for child in element)

    return ModMetaData(
        mod_id,
        mod_name,
        mod_version,
        mod_url,
        mod_description,
        mod_supported_versions,
        mod_authors,
        mod_dependencies,
    )
