from pathlib import Path
from xml.etree import ElementTree

from cli.utils import SemanticVersion
from package import Package

type Url = str
type PackagePattern = str


def read_package_properties(file: Path) -> list[Package]:
    container: list[Package] = []

    with file.open("r") as f:
        tree = ElementTree.parse(f)
        root = tree.getroot()

        for element in root.iterfind("ItemGroup/PackageVersion"):
            container.append(
                Package(
                    element.attrib["Include"],
                    SemanticVersion.parse_version(element.attrib["Version"]),
                )
            )

    return container


def read_nuget_config(file: Path) -> dict[Url, list[PackagePattern]]:
    """
    Reads a NuGet configuration file and parses the package source mappings into
    a dictionary structure. The configuration file should conform to the NuGet
    configuration schema, containing `packageSources` and `packageSourceMapping`
    elements.

    This function returns a mapping where each key is a URL corresponding to a
    package source, and the value is a list of package patterns associated with
    that source.

    :param file: The path to the NuGet configuration file to read.
    :returns: A dictionary mapping package source URLs to lists of package patterns.
    """
    type SourceKey = str

    source_map: dict[SourceKey, Url] = {}
    container: dict[Url, list[PackagePattern]] = {}

    with file.open("r") as f:
        tree = ElementTree.parse(f)
        root = tree.getroot()

        for element in root.iterfind("packageSources/add"):
            source_map[element.attrib["key"]] = element.attrib["value"]
            container[source_map[element.attrib["key"]]] = []

        for element in root.iterfind("packageSourceMapping/packageSource"):
            source_url = source_map[element.attrib["key"]]
            container[source_url].extend(
                [package.attrib["pattern"] for package in element.iterfind("package")]
            )

    return container


if __name__ == "__main__":
    import pprint

    pprint.pprint(
        read_package_properties(Path.cwd().joinpath("Directory.Packages.props"))
    )
    pprint.pprint(read_nuget_config(Path.cwd().joinpath("nuget.config")))
