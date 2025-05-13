import re
from functools import total_ordering

__all__ = ["SemanticVersion"]


@total_ordering
class SemanticVersion:
    """
    Represents a semantic version compliant with the Semantic Versioning specification.

    This class provides mechanisms to represent, compare, and parse versions in the
    Semantic Versioning (SemVer) format. It adheres to the standard specification that 
    includes major, minor, and optional patch version numbers, as well as optional 
    pre-release and build metadata. Instances of this class can be compared using the 
    `<`, `==`, and other comparison operators.

    Attributes:
        SEMVER_REGEX (str):
            A regular expression pattern for validating and extracting components from 
            a semantic version string.
        major (int):
            The major version number, representing incompatible changes.
        minor (int):
            The minor version number, representing added functionality in a
            backward-compatible manner.
        patch (int):
            The patch version number, identifying backward-compatible bug fixes.
        pre_release (Optional[str]):
            Pre-release version information, indicating a development version.
        build_metadata (Optional[str]):
            Build metadata, providing additional version information.
    """

    SEMVER_REGEX = re.compile(
        r"^(?P<major>0|[1-9]\d*)\.(?P<minor>0|[1-9]\d*)"
        r"(\.(?P<patch>0|[1-9]\d*))?"
        r"(-(?P<pre_release>[\w.-]+))?"
        r"(\+(?P<build_metadata>[\w.-]+))?$"
    )

    def __init__(self, major, minor, patch=0, pre_release=None, build_metadata=None):
        self.major = int(major)
        self.minor = int(minor)
        self.patch = int(patch)
        self.pre_release = pre_release
        self.build_metadata = build_metadata

    @staticmethod
    def parse_version(version_str):
        """
        Parses a semantic version string according to the Semantic Versioning specification.

        Utilizes a regular expression pattern defined in SEMVER_REGEX to validate and extract
        the components of the version string. If the input string does not conform to the
        Semantic Versioning format, a ValueError is raised.

        Args:
            version_str (str): A string representing the semantic version to be parsed.

        Returns:
            SemanticVersion: An instance of the SemanticVersion class, containing the components
            of the parsed version (major, minor, patch, pre-release, build metadata).

        Raises:
            ValueError: If the input string does not conform to the Semantic Versioning format.
        """
        match = SemanticVersion.SEMVER_REGEX.match(version_str)
        
        if not match:
            raise ValueError(f"Invalid semantic version: {version_str}")

        groups = match.groupdict()
        return SemanticVersion(
            groups["major"],
            groups["minor"],
            groups.get("patch", "0"),
            groups.get("pre_release"),
            groups.get("build_metadata"),
        )

    def __eq__(self, other):
        if not isinstance(other, SemanticVersion):
            return NotImplemented
        return (
            self.major == other.major
            and self.minor == other.minor
            and self.patch == other.patch
            and self.pre_release == other.pre_release
        )

    def __lt__(self, other):
        if not isinstance(other, SemanticVersion):
            return NotImplemented

        if (self.major, self.minor, self.patch) != (
            other.major,
            other.minor,
            other.patch,
        ):
            return (self.major, self.minor, self.patch) < (
                other.major,
                other.minor,
                other.patch,
            )

        return self._compare_pre_release(self.pre_release, other.pre_release) < 0

    @staticmethod
    def _compare_pre_release(pr1, pr2):
        if pr1 is None:
            return 1 if pr2 else 0
        if pr2 is None:
            return -1

        pr1_parts = pr1.split(".")
        pr2_parts = pr2.split(".")
        for sub1, sub2 in zip(pr1_parts, pr2_parts):
            try:
                result = (int(sub1) if sub1.isdigit() else sub1) < (
                    int(sub2) if sub2.isdigit() else sub2
                )
                if result:
                    return -1
                if result > 0:
                    return 1
            except ValueError:
                pass

        return len(pr1_parts) - len(pr2_parts)

    def __str__(self):
        version = f"{self.major}.{self.minor}.{self.patch}"
        if self.pre_release:
            version += f"-{self.pre_release}"
        if self.build_metadata:
            version += f"+{self.build_metadata}"
        return version

    def __repr__(self):
        return f"{self.__class__.__name__}({self})"
