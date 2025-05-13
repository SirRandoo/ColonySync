from dataclasses import dataclass
from typing import Self

from ..utils.versioning import SemanticVersion
from yarl import URL

__all__ = [
    "SearchQueryResponse",
    "SearchQueryContext",
    "SearchQueryDataEntry",
    "SearchQueryPackageVersion",
    "SearchQueryPackageType",
]


@dataclass(slots=True)
class SearchQueryContext:
    """
    Represents a context for a search query, typically including base and vocabulary URLs.

    This class is designed to encapsulate the context of a search query, which often includes
    details about a base URL and a vocabulary (vocab) URL. It provides functionality to
    serialize and deserialize the context from and to JSON format.

    Attributes:
        base (URL): The base URL associated with the search query context.
        vocab (URL): The vocabulary URL associated with the search query context.
    """

    base: URL
    vocab: URL

    @classmethod
    def from_json(cls, data: dict) -> Self:
        """
        Creates a new instance of SearchQueryContext from a JSON-like dictionary.

        This method parses a dictionary containing specific key-value pairs into
        a SearchQueryContext instance by extracting and converting the appropriate
        values into corresponding attributes. It assumes that the data follows a
        specific structure, where predefined keys map to values that are essential
        for constructing the instance. The keys '@base' and '@vocab' should map
        to valid URL strings.

        Args:
            data:
                Dictionary containing the JSON representation of the SearchQueryContext.
                The dictionary must have the keys '@base' and '@vocab', with respective
                values that can be converted into URL objects.

        Returns:
            An instance of the SearchQueryContext class with properties initialized
            based on the input data.
        """
        return SearchQueryContext(
            base=URL(data["@base"]),
            vocab=URL(data["@vocab"]),
        )

    def to_json(self) -> dict:
        """
        Converts the instance properties `base` and `vocab` to a JSON-serializable dictionary.

        This method is used to represent the object in a JSON-compatible
        format, converting the `base` and `vocab` attributes to string values
        and organizing them with specific keys.

        Returns:
            dict:
                A dictionary containing the `base` and `vocab` attributes
                as key-value pairs, with the keys prefixed by "@".
        """
        return {
            "@base": str(self.base),
            "@vocab": str(self.vocab),
        }


@dataclass(slots=True)
class SearchQueryPackageType:
    """Representation of a search query package type.

    This class is designed to encapsulate a search query package type with
    attributes and utility functions to support serialization and deserialization
    to and from JSON format. It includes methods to easily transform the class
    instance to a dictionary and construct an instance from a dictionary, enabling
    effective interaction with JSON-based data.

    Attributes:
        name (str): The name of the search query package type.
    """

    name: str

    @classmethod
    def from_json(cls, data: dict) -> Self:
        """
        Creates an instance of the class from a JSON dictionary representation.

        This class method is designed to construct an instance of the class by parsing
        a given dictionary that represents the JSON object. It maps the values found
        in the provided dictionary to the expected attributes of the class.

        Args:
            data (dict):
                A dictionary representing the JSON-encoded object. This dictionary
                must contain a "name" key with a value that corresponds to the desired
                name for the SearchQueryPackageType instance.

        Returns:
            Self: An instance of the class initialized using the provided JSON data.
        """
        return SearchQueryPackageType(data["name"])

    def to_json(self) -> dict:
        """
        Converts the instance attributes into a dictionary format suitable for JSON serialization.

        This method creates a dictionary containing specific instance attributes and their
        corresponding values, making it ready to be serialized into a JSON object. The keys
        of the returned dictionary represent the attribute names.

        Returns:
            dict: A dictionary representation of specific instance attributes.
        """
        return {
            "name": self.name,
        }


@dataclass(slots=True)
class SearchQueryPackageVersion:
    """
    Represents a package version in a search query package response.

    This class encapsulates the details of a specific package version, including
    its unique identifier, the number of downloads it has, and its version
    information. It provides functionality to serialize and deserialize its data
    to and from JSON format.

    Attributes:
        id (URL): The unique resource identifier of the package version.
        downloads (int): The total number of downloads for the package version.
        version (Version): The version information of the package.
    """

    id: URL
    downloads: int
    version: SemanticVersion

    @classmethod
    def from_json(cls, data: dict) -> Self:
        """
        Converts a dictionary representation (in JSON format) to an instance of the
        SearchQueryPackageVersion class.

        This method is used to parse and construct a `SearchQueryPackageVersion` object
        from a JSON object passed as a Python dictionary. It extracts specific fields
        required for the object's initialization, such as the ID, download count, and
        version information.

        Args:
            data (dict):
                A dictionary representing the JSON object, where `@id`
                corresponds to the ID of the package, `downloads` is the download count,
                and `version` is the version string of the package.

        Returns:
            Self: A new instance of the SearchQueryPackageVersion class populated with
            data derived from the provided JSON dictionary.
        """
        return SearchQueryPackageVersion(
            id=data["@id"],
            downloads=data["downloads"],
            version=SemanticVersion.parse_version(data["version"]),
        )

    def to_json(self) -> dict:
        """
        Converts the object to a JSON serializable dictionary.

        This method creates a dictionary representation of the object, which includes
        specific attributes transformed to string format where necessary. The resulting
        dictionary can be used for JSON serialization.

        Returns:
            dict: A dictionary containing the object's data, formatted for JSON output.
        """
        return {
            "@id": str(self.id),
            "downloads": self.downloads,
            "version": str(self.version),
        }


@dataclass(slots=True)
class SearchQueryDataEntry:
    """
    Represents an entry in the search query data.

    This class encapsulates the essential information about a package or project
    retrieved from a search query in a structured format. It includes details such
    as metadata, authors, license information, version history, and associated
    vulnerabilities. It allows easy conversion between JSON data and its Python
    representation.

    Attributes:
        id_url (URL): The unique URL identifier for the entry.
        type (str): The type or category of the entry.
        authors (list[str]): A list of authors associated with the entry.
        description (str): A textual description of the entry.
        icon_url (URL): The URL for the entry's icon.
        id (str): The unique identifier for the entry.
        license_url (URL): The URL linking to the license of the package/project.
        owners (list[str]): A list of those who own or maintain the entries.
        package_types (list[SearchQueryPackageType]):
            Types or frameworks of the package, parsed into structured objects.
        project_url (URL): The URL of the project's main page or repository.
        registration (URL): The registration URL for the package or project.
        summary (str): A brief summary describing the entry.
        tags (list[str]): A collection of tags associated with the entry.
        title (str): The title or name of the entry.
        total_downloads (int): The total number of downloads for the entry.
        verified (bool): Indicates whether the entry is verified.
        version (Version): Represents the current version of the entry.
        versions (list[SearchQueryPackageVersion]):
            The list of version details for the entry, represented as structured objects.
        vulnerabilities (list):
            A collection of vulnerability details associated with the entry.
    """

    id_url: URL
    type: str
    authors: list[str]
    description: str
    icon_url: URL
    id: str
    license_url: URL
    owners: list[str]
    package_types: list[SearchQueryPackageType]
    project_url: URL
    registration: URL
    summary: str
    tags: list[str]
    title: str
    total_downloads: int
    verified: bool
    version: SemanticVersion
    versions: list[SearchQueryPackageVersion]
    vulnerabilities: list

    @classmethod
    def from_json(cls, data: dict) -> Self:
        """
        Creates an instance of the class from a JSON dictionary.

        This method parses the provided JSON dictionary to create and return an
        instance of the class. It extracts the relevant fields, initializes
        properties, and transforms the data into suitable formats such as
        lists, custom data structures, and nested class instances.

        Args:
            data (dict):
                A JSON dictionary containing all the necessary fields
                to instantiate the class. It must include keys like '@id',
                '@type', 'authors', 'description', 'iconUrl', etc., with
                corresponding values that conform to the expected format and
                type requirements.

        Returns:
            Self:
                An instance of the current class constructed from the input data.
        """
        return SearchQueryDataEntry(
            id_url=URL(data["@id"]),
            type=data["@type"],
            authors=data["authors"],
            description=data["description"],
            icon_url=URL(data["iconUrl"]),
            id=data["id"],
            owners=data["owners"],
            package_types=[
                SearchQueryPackageType.from_json(package_type)
                for package_type in data["packageTypes"]
            ],
            project_url=URL(data["projectUrl"]),
            license_url=URL(data["licenseUrl"]),
            registration=URL(data["registration"]),
            summary=data["summary"],
            tags=data["tags"],
            title=data["title"],
            total_downloads=data["totalDownloads"],
            verified=data["verified"],
            version=SemanticVersion.parse_version(data["version"]),
            versions=[
                SearchQueryPackageVersion.from_json(version)
                for version in data["versions"]
            ],
            vulnerabilities=data["vulnerabilities"],
        )

    def to_json(self) -> dict:
        """
        Converts the object into a dictionary representation suitable for JSON serialization.

        This method generates a structured dictionary containing attributes of the object,
        allowing for a consistent JSON serialization output. It includes key fields such as
        identifiers, URLs, metadata, version details, and security vulnerabilities. Nested
        attributes like package types and versions are also serialized using individual
        `to_json` methods to ensure proper formatting.

        Returns:
            dict: A dictionary representation of the object, suitable for JSON serialization.
        """
        return {
            "@id": str(self.id_url),
            "@type": self.type,
            "authors": self.authors,
            "description": self.description,
            "iconUrl": str(self.icon_url),
            "id": self.id,
            "licenseUrl": str(self.license_url),
            "owners": self.owners,
            "packageTypes": [type_.to_json() for type_ in self.package_types],
            "projectUrl": str(self.project_url),
            "registration": str(self.registration),
            "summary": self.summary,
            "tags": self.tags,
            "title": self.title,
            "totalDownloads": self.total_downloads,
            "verified": self.verified,
            "version": str(self.version),
            "versions": [version.to_json() for version in self.versions],
            "vulnerabilities": self.vulnerabilities,
        }


@dataclass(slots=True)
class SearchQueryResponse:
    """
    Represents the response object for a search query.

    This class is used to store the result of a search query, including the
    query context, the data that matches the query, and the total number of
    hits. It allows converting the response data between JSON format and
    class instances for easier operation and manipulation.

    Attributes:
        context (SearchQueryContext):
            The contextual information related to the search query.
        data (list[SearchQueryDataEntry]):
            A list of entries that match the search query.
        total_hits (int): The total number of results for the search query.
    """

    context: SearchQueryContext
    data: list[SearchQueryDataEntry]
    total_hits: int

    @classmethod
    def from_json(cls, data: dict) -> Self:
        """
        Parses a JSON object into an instance of the class.

        This method serves as a factory for creating class instances directly from
        JSON data. It extracts and maps the required values from the provided data
        structure into the respective attributes of the class, ensuring proper
        initialization of nested objects.

        Args:
            data (dict):
                A dictionary containing JSON data to be parsed. The input
                must adhere to the expected JSON schema for proper deserialization.

        Returns:
            Self:
                An initialized instance of the class, populated with data parsed
                from the given JSON object.
        """
        return SearchQueryResponse(
            context=SearchQueryContext.from_json(data["@context"]),
            data=[
                SearchQueryDataEntry.from_json(data_entry)
                for data_entry in data["data"]
            ],
            total_hits=data["totalHits"],
        )

    def to_json(self) -> dict:
        """
        Converts the object instance into a JSON-compatible dictionary format.

        This method serializes the object into a dictionary that adheres to a specific
        structure, converting individual attributes or components of the object into
        JSON-formatted strings or lists where applicable. Nested objects are also
        converted using their respective `to_json` methods.

        Returns:
            dict:
                A dictionary representation of the object suitable for JSON serialization.
        """
        return {
            "@context": str(self.context.to_json()),
            "data": [data_entry.to_json() for data_entry in self.data],
            "totalHits": self.total_hits,
        }
