from io import TextIOWrapper
from xml.etree import cElementTree as ET
from .version import Version


def parse_build_props(file: TextIOWrapper) -> dict[str, str]:
    """Parses an XML input file and extracts properties from the "PropertyGroup" tags.

    The function reads an XML file and constructs a dictionary where each key represents a tag
    name and its corresponding value represents the text content of that tag found within
    "PropertyGroup" elements.

    Args:
        file: A file-like object representing the XML file to parse.
    Returns:
        A dictionary where keys are the tag names from elements inside "PropertyGroup" and values
        are their associated text content.
    """
    tree = ET.parse(file)
    root = tree.getroot()

    container: dict[str, str] = {}

    for element in root.iterfind("PropertyGroup"):
        container[element.tag] = element.text

    return container


def parse_packages_props(file: TextIOWrapper) -> dict[str, Version]:
    """Parses the given file to extract package properties and their versions.

    This function reads an XML file that specifies package details and versions,
    parses it, and returns a dictionary containing package names as keys and their
    corresponding version objects as values.

    Args:
        file: A readable file stream containing the XML data.
    Returns:
        A dictionary where the keys are package names and the values are their
        corresponding Version objects.
    """
    tree = ET.parse(file)
    root = tree.getroot()

    container: dict[str, Version] = {}

    for element in root.iterfind("ItemGroup/PackageVersion"):
        container[element.attrib["Include"]] = Version.parse(element.attrib["Version"])

    return container
