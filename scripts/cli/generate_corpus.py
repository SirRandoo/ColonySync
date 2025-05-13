from typing import Self

import orjson as json
from logging import Logger
from logging import getLogger
from pathlib import Path
from typing import Final
from urllib.request import urlopen
from xml.etree import ElementTree as ET
from dataclasses import dataclass
from dataclasses import field
from cli.utils import SemanticVersion

from cli.nuget import NuGetClient

_LOGGER: Final[Logger] = getLogger(__name__)

type Url = str
type VersionString = str
type NuGetConfigPattern = str

_NUGET_CLIENT: Final[NuGetClient] = NuGetClient()


@dataclass(slots=True)
class Package:
    name: str
    version: SemanticVersion


@dataclass(slots=True)
class Project:
    name: str
    version: SemanticVersion
    project_dependencies: list[Self] = field(default_factory=list)
    package_dependencies: list[Package] = field(default_factory=list)


def _read_nuget_config(nuget_config_path: Path) -> dict[Url, list[NuGetConfigPattern]]:
    sources_map: dict[str, Url] = {}
    config: dict[Url, list[NuGetConfigPattern]] = {}

    with nuget_config_path.open("r") as f:
        root = ET.parse(f).getroot()

        for node in root:
            match node.tag:
                case "packageSource":
                    sources_map = {
                        n.attrib["key"]: n.attrib["value"]
                        for n in node
                        if n.tag == "add"
                    }
                    config.update({url: [] for url in sources_map.values()})
                case "packageSourceMapping":
                    for source in node:
                        if source.tag != "packageSource":
                            continue
                        source_url = sources_map[source.attrib["key"]]
                        config[source_url].append(source.attrib["pattern"])

    return config


def _read_packages_properties(
    package_properties_file: Path,
) -> dict[str, VersionString]:
    properties: dict[str, str] = {}

    with package_properties_file.open("r") as f:
        tree = ET.parse(f)
        root = tree.getroot()

        for node in root:
            if node.tag != "ItemGroup":
                continue

            properties.update(
                {
                    n.attrib["Include"]: n.attrib.get("Version", "")
                    for n in node
                    if n.tag == "PackageVersion"
                }
            )

    return properties


def _read_project_file(
    project_file: Path, *, version_mapping: dict[str, str] | None = None
) -> dict[str, str]:
    properties: dict[str, str] = {}

    with project_file.open("r") as f:
        tree = ET.parse(f)
        root = tree.getroot()

        for node in root:
            if node.tag != "ItemGroup":
                continue

            properties.update(
                {
                    n.attrib["Include"]: n.attrib.get("Version", None)
                    for n in node
                    if n.tag == "PackageReference"
                }
            )

            if version_mapping:
                for key, value in properties.items():
                    if key in version_mapping and value is None:
                        properties[key] = version_mapping[key]

    return properties


type Project = str
type Dependency = str
type DependencyGraph = dict[Project, list[Dependency]]


def _generate_dependency_graph(source_root: Path) -> DependencyGraph:
    container: DependencyGraph = {}

    for directory, directories, files in source_root.walk():
        for file in files:
            if not file.endswith(".csproj"):
                continue

            packages = _read_project_file(directory.joinpath(file))

    return container


def generate_corpus(source_root: Path):
    pass


if __name__ == "__main__":
    _mapping: dict[str, str] = _read_packages_properties(
        Path.cwd().joinpath("Directory.Packages.props")
    )
