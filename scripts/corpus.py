from dataclasses import dataclass
from dataclasses import field
from enum import StrEnum
from pathlib import Path
from xml.etree import cElementTree as ET


class ResourceType(StrEnum):
    """

    ResourceType is an enumerated type representing different types of resource identifiers in the .NET environment.

    Attributes:
        NET_STANDARD_ASSEMBLY (str): Represents a .NET standard assembly.
        ASSEMBLY (str): Represents a general assembly.
        DLL (str): Represents a DLL (Dynamic Link Library) file.
    """

    NET_STANDARD_ASSEMBLY = "NetStandardAssembly".casefold()
    ASSEMBLY = "Assembly".casefold()
    DLL = "Dll".casefold()


@dataclass(slots=True)
class Resource:
    """
    Resource class representing a resource entity.

    Attributes:
        type (ResourceType): The type of the resource.
        name (str): The name of the resource.
        optional (bool): Flag indicating whether the resource is optional. Defaults to False.
        root (str | None): Root directory path for the resource. Defaults to None.
    """

    type: ResourceType
    name: str
    optional: bool = field(default=False)
    root: str | None = field(default=None)


@dataclass(slots=True)
class ResourceBundle:
    """
    ResourceBundle represents a collection of resources managed as a bundle.

    Attributes:
        root (Path): The root directory path containing the resources.
        resources (list[Resource]): A list of resource objects included in the bundle.
        versioned (bool, optional): Indicates if the resource bundle is versioned. Defaults to False.
    """

    root: Path
    resources: list[Resource]
    versioned: bool = field(default=False)


@dataclass(slots=True)
class Corpus:
    """
    Represents a collection of resource bundles.

    Attributes:
        bundles (list[ResourceBundle]): A list containing instances of ResourceBundle.

    """

    bundles: list[ResourceBundle]


def load_corpus(path: Path) -> Corpus:
    """
    Parses an XML file located at the given path and returns a Corpus object. The XML file should contain resource bundles within a 'Resources' root element.

    Args:
        path: Path to the XML file that contains the corpus data.

    Returns:
        A Corpus object containing parsed resource bundles and their corresponding resources.

    Raises:
        ValueError: If the XML file is malformed, specifically if the 'Resources' element is missing within the 'Corpus' element.
    """
    with path.open("r", encoding="utf-8") as file:
        xml = ET.ElementTree(file=file)

    resources_root = xml.getroot().find("Resources")

    if resources_root is None:
        raise ValueError(
            "Corpus file is malformed, and should contain a root 'Resources' element within the 'Corpus' element"
        )

    bundles: list[ResourceBundle] = []
    for bundle in resources_root:
        bundle_root = Path(bundle.attrib["Root"])
        bundle_versioned = (
            True
            if bundle.attrib.get("Versioned", "false").casefold() == "true".casefold()
            else False
        )

        bundle_obj = ResourceBundle(bundle_root, [], bundle_versioned)
        bundles.append(bundle_obj)

        for resource in bundle:
            resource_type = ResourceType(resource.attrib["Type"].casefold())
            resource_name = resource.attrib["Name"]
            resource_optional = (
                True
                if resource.attrib.get("Optional", "false").casefold()
                == "true".casefold()
                else False
            )
            resource_root = resource.attrib.get("Root", None)

            bundle_obj.resources.append(
                Resource(resource_type, resource_name, resource_optional, resource_root)
            )

    return Corpus(bundles)
