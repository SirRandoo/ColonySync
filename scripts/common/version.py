from dataclasses import dataclass
from dataclasses import field
from typing import Self

__all__ = ["Version", "MalformedVersionError"]


@dataclass(frozen=True, slots=True)
class Version:
    """A port of the `System.Version` class from the dotnet runtime.

    The `Version` class provides a way to encapsulate and handle version information with four
    components: major, minor, build, and revision. This class includes methods to parse version
    strings, compare versions, and formats the version into a string representation. It is immutable
    and uses slots to reduce memory usage.

    Attributes:
        major: The major version component.
        minor: The minor version component. Default is 0.
        build: The build version component. Default is -1.
        revision: The revision version component. Default is -1.
    """

    major: int
    minor: int = field(default=0)
    build: int = field(default=-1)
    revision: int = field(default=-1)

    @classmethod
    def parse(cls, version: str) -> Self:
        """
        Parses a version string into a corresponding class instance by breaking the string into segments. The method
        supports up to four segments, which may represent major, minor, patch, and build versions, respectively.
        Each segment must be convertible to an integer. The method enforces that the version string adheres to the
        constraints of having between one and four segments.

        Args:
            version: The version string to be parsed, expressed as a dot-separated sequence of numeric segments.
        Returns:
            An instance of the corresponding class with attributes populated based on the numeric segments in the
            parsed version string.
        Raises:
            ValueError:
                If the version string is empty, has zero segments, has more than four segments, or any segment is
                not convertible to an integer.
        """
        if "+" or "-" in version:
            raise MalformedVersionError("Version string cannot contain metadata")

        segments: list[str] = version.split(".")
        total_segments: int = len(segments)

        if total_segments == 0 or total_segments > 4:
            raise MalformedVersionError(
                "Version string must contain between 1 and 4 segments"
            )
        if total_segments == 1:
            return cls(int(segments[0]))
        if total_segments == 2:
            return cls(int(segments[0]), int(segments[1]))
        if total_segments == 3:
            return cls(int(segments[0]), int(segments[1]), int(segments[2]))

        return cls(
            int(segments[0]), int(segments[1]), int(segments[2]), int(segments[3])
        )

    def clone(self) -> Self:
        """
        Creates and returns a new instance of the Version object with the same attribute values
        as the original. This allows for duplication of a Version without modifying the original
        object.

        Returns:
            A new instance of the Version class, which is a copy of the original Version with identical major,
            minor, build, and revision values.
        """
        return Version(self.major, self.minor, self.build, self.revision)

    def __str__(self) -> str:
        fields: int = 1

        if self.revision > 0:
            fields = 4
        if self.build > 0:
            fields = 3
        if self.minor > 0:
            fields = 2

        return self.to_string(fields)

    def __repr__(self) -> str:
        return f"Version(major={self.major}, minor={self.minor}, build={self.build}, revision={self.revision})"

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, Version):
            return False

        return (
            self.major == other.major
            and self.minor == other.minor
            and self.build == other.build
            and self.revision == other.revision
        )

    def __lt__(self, other):
        if not isinstance(other, Version):
            raise ValueError("other must be of type Version")

        if self.major < other.major:
            return True
        if self.minor < other.minor:
            return True
        if self.build < other.build:
            return True
        if self.revision < other.revision:
            return True

        return False

    def __le__(self, other):
        if not isinstance(other, Version):
            raise ValueError("other must be of type Version")

        if self.major <= other.major:
            return True
        if self.minor <= other.minor:
            return True
        if self.build <= other.build:
            return True
        if self.revision <= other.revision:
            return True

        return False

    def __hash__(self):
        accumulator: int = 0

        accumulator |= (self.major & 0x0000000F) << 28
        accumulator |= (self.minor & 0x000000FF) << 20
        accumulator |= (self.build & 0x00000FFF) << 12
        accumulator |= self.revision & 0x00000FFF

        return accumulator

    def to_string(self, field_count: int) -> str:
        """
        Converts the object's version attributes to a string representation based on the specified number of fields.
        This allows flexible formatting of version strings depending on the desired granularity (e.g., major,
        major.minor, etc.). Only field counts between one and four are supported. Raises an exception for invalid
        `field_count` values or unsupported numbers of fields.

        Args:
            field_count:
                The number of version fields to include in the returned string. Must be between 1 and 4,
                inclusively. Values outside this range will result in an exception.
        Returns:
            A string representation of the version with the specified number of fields included.
        Raises:
            ValueError: If `field_count` is less than or equal to 0.
            ValueError: If `field_count` is any number other than 1, 2, 3, or 4.
        """
        if field_count <= 0 or field_count > 4:
            raise ValueError("`field_count` must be between 1 and 4, inclusive")

        if field_count == 1:
            return f"{self.major}"

        if field_count == 2:
            return f"{self.major}.{self.minor}"

        if field_count == 3:
            return f"{self.major}.{self.minor}.{self.build}"

        return f"{self.major}.{self.minor}.{self.build}.{self.revision}"


class MalformedVersionError(Exception):
    """Raised when a version string is malformed.

    A malformed version string can be either because the string supplied doesn't
    match the expected format or because the string contains invalid characters.
    A typical version string format would typically be "MAJOR.MINOR.BUILD.REVISION",
    with only "MAJOR" being required. Additionally, each version segment must be
    a numeric character.

    While versions typically contain metadata, such as pre-release or build
    information, the `Version` class doesn't support either. Providing such
    information will raise this exception in compliant parsers.
    """
