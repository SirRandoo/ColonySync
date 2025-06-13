from dataclasses import dataclass
from dataclasses import replace
from io import TextIOWrapper
from logging import getLogger
from logging import Logger
from typing import Final
from xml.etree import cElementTree as ET

from .version import Version

__all__ = ["NuSpecDependency", "NuSpecDependencyGroup", "NuSpec", "load_nuspec"]


_LOGGER: Final[Logger] = getLogger(__name__)


@dataclass(slots=True, frozen=True)
class NuSpecDependency:
    id: str
    version: Version


@dataclass(slots=True, frozen=True)
class NuSpecDependencyGroup:
    target_framework: str
    dependencies: tuple[NuSpecDependency, ...]


@dataclass(slots=True, frozen=True)
class NuSpec:
    id: str
    version: Version
    title: str
    authors: tuple[str, ...]
    require_license_acceptance: bool
    license: str
    license_type: str
    license_url: str
    icon: str
    readme: str
    project_url: str
    icon_url: str
    description: str
    copyright: str
    tags: tuple[str, ...]
    dependencies: tuple[NuSpecDependencyGroup, ...]


def _parse_authors(author_string: str) -> tuple[str, ...]:
    authors: tuple[str, ...] = tuple()

    if ";" in author_string:
        authors = tuple(author.strip() for author in author_string.split(";"))
    elif "," in author_string:
        authors = tuple(author.strip() for author in author_string.split(","))
    else:
        authors = tuple(author.strip() for author in author_string.split(" ") if author)

    if authors and "and " in authors[-1]:
        authors = tuple(authors[:-1]) + authors[-1][len("and ") :]

    return authors


def _parse_dependency_group(element: ET.Element) -> tuple[NuSpecDependencyGroup, ...]:
    dependency_groups: tuple[NuSpecDependencyGroup, ...] = tuple()

    if element.tag != "group":
        _LOGGER.warning(
            "Attempted to parse dependency group, but format is "
            "unrecognized; expected tag 'group' but received '%s'",
            element.tag,
        )

        raise ValueError("Unexpected dependency group format")

    for dependency_group in element.iter():
        if dependency_group.tag != "group":
            continue

        target_framework: str = dependency_group.attrib.get("targetFramework")

        dependency_group_instance: NuSpecDependencyGroup = NuSpecDependencyGroup(
            target_framework=target_framework,
            dependencies=tuple(
                NuSpecDependency(
                    dependency.attrib["id"], Version.parse(dependency.attrib["version"])
                )
                for dependency in dependency_group.iter()
                if dependency.tag == "dependency"
            ),
        )

        dependency_groups = dependency_groups + dependency_group_instance

    return dependency_groups


def load_nuspec(file: TextIOWrapper) -> NuSpec:
    tree: ET.ElementTree = ET.parse(file)
    root: ET.Element = tree.getroot()
    metadata_element: ET.Element = root.find("metadata")

    nuspec_instance: NuSpec = NuSpec(
        "",
        Version(0, 0, 0),
        "",
        tuple(),
        False,
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        tuple(),
        tuple(),
    )

    for element in metadata_element.iter():
        match element.tag:
            case "id":
                nuspec_instance = replace(nuspec_instance, id=element.text)
            case "version":
                nuspec_instance = replace(
                    nuspec_instance, version=Version.parse(element.text)
                )
            case "title":
                nuspec_instance = replace(nuspec_instance, title=element.text)
            case "authors":
                nuspec_instance = replace(
                    nuspec_instance, authors=_parse_authors(element.text)
                )
            case "requireLicenseAcceptance":
                nuspec_instance = replace(
                    nuspec_instance, require_license_acceptance=element.text == "true"
                )
            case "license":
                nuspec_instance = replace(
                    nuspec_instance,
                    license=element.text,
                    license_type=element.attrib.get("type"),
                )
            case "licenseUrl":
                nuspec_instance = replace(nuspec_instance, license_url=element.text)
            case "icon":
                nuspec_instance = replace(nuspec_instance, icon=element.text)
            case "readme":
                nuspec_instance = replace(nuspec_instance, readme=element.text)
            case "projectUrl":
                nuspec_instance = replace(nuspec_instance, project_url=element.text)
            case "iconUrl":
                nuspec_instance = replace(nuspec_instance, icon_url=element.text)
            case "description":
                nuspec_instance = replace(nuspec_instance, description=element.text)
            case "copyright":
                nuspec_instance = replace(nuspec_instance, copyright=element.text)
            case "tags":
                nuspec_instance = replace(
                    nuspec_instance, tags=tuple(element.text.split(" "))
                )
            case "dependencies":
                nuspec_instance = replace(
                    nuspec_instance, dependencies=_parse_dependency_group(element)
                )

    return nuspec_instance
