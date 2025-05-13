from dataclasses import dataclass
from dataclasses import field
from typing import Final
from typing import Self
from typing import TypedDict
from cli.utils import SemanticVersion

__all__ = ["Package", "serialize_package", "deserialize_package"]

_PACKAGE_CACHE: Final[dict[str, "Package"]] = {}
_SERIALIZED_PACKAGE_CACHE: Final[dict["Package", "_SerializedPackage"]] = {}


class _SerializedPackage(TypedDict):
    name: str
    version: SemanticVersion
    dependencies: list[Self]


@dataclass(slots=True)
class Package:
    """Represents a software package with specific attributes.

    This class is used to encapsulate information about a software package,
    including its name, version, and any dependencies it has on other
    packages. Dependencies are structured recursively, allowing a package
    to define other packages it relies on for functionality.

    :param name: The name of the software package.
    :param version: The version of the software package.
    :param file_names: The name of files included in the package.
    :param dependencies: A list of other `Package` instances that this package depends on.
    """

    name: str
    version: SemanticVersion
    file_names: set[str] = field(default_factory=set)
    dependencies: list[Self] = field(default_factory=list)


def serialize_package(package: Package) -> _SerializedPackage:
    """Serializes a package into a JSON object.

    :param package: The package to be serialized.
    :return: A JSON object representing the serialized package.
    """
    dependencies: list[_SerializedPackage] = []

    for dependency in package.dependencies:
        cached_package = _SERIALIZED_PACKAGE_CACHE.get(dependency, None)

        if cached_package:
            dependencies.append(cached_package)
        else:
            _SERIALIZED_PACKAGE_CACHE[dependency] = serialize_package(dependency)
            dependencies.append(_SERIALIZED_PACKAGE_CACHE[dependency])

    return {
        "name": package.name,
        "version": package.version,
        "dependencies": dependencies,
    }


def deserialize_package(package: _SerializedPackage) -> Package:
    """Deserializes a JSON object into a `Package`.

    Args:
        package:
            The JSON object representing the package.
    Returns:
        The deserialized package as an instance of `Package`.
    """
    dependencies: list[Package] = []

    for dependency in dependencies:
        cached_package = _PACKAGE_CACHE.get(dependency.name, None)

        if cached_package:
            dependencies.append(cached_package)
        else:
            dependencies.append(dependency)
            _PACKAGE_CACHE[dependency.name] = dependency

    return Package(
        package["name"],
        package["version"],
        [deserialize_package(dep) for dep in package["dependencies"]],
    )
