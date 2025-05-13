from typing import Self


class Version:
    """
    Represents a semantic version and provides utilities for version comparison and manipulation.

    This class is used to work with semantic versioning, which follows the
    format "MAJOR.MINOR.PATCH". It allows for parsing versions from strings,
    comparison between versions, and modifying specific parts (major, minor,
    or patch) of a version. This is useful in managing versioned software or
    libraries.

    Attributes:
        major (int): The major version number.
        minor (int): The minor version number.
        patch (int): The patch version number.
    """

    def __init__(self, version: str):
        parts = version.split(".")

        if len(parts) != 3 or not all(part.isdigit() for part in parts):
            raise ValueError("Invalid semantic version format, expected 'MAJOR.MINOR.PATCH'")

        self.major, self.minor, self.patch = map(int, parts)

    def __str__(self) -> str:
        return f"{self.major}.{self.minor}.{self.patch}"

    def __eq__(self, other: Self) -> bool:
        return (self.major, self.minor, self.patch) == (other.major, other.minor, other.patch)

    def __lt__(self, other: Self) -> bool:
        return (self.major, self.minor, self.patch) < (other.major, other.minor, other.patch)

    def __le__(self, other: Self) -> bool:
        return self < other or self == other

    def __gt__(self, other: Self) -> bool:
        return not (self <= other)

    def __ge__(self, other: Self) -> bool:
        return not (self < other)

    def increment(self, part: str) -> None:
        """
        Increments the version number based on the specified part. The version is
        assumed to follow semantic versioning with `major`, `minor`, and `patch`
        levels. Incrementing `major` resets `minor` and `patch` to 0. Incrementing
        `minor` resets `patch` to 0. Incrementing `patch` only affects the patch
        level.

        Args:
            part (str): The part of the version to increment. Valid values are
                'major', 'minor', or 'patch'.

        Raises:
            ValueError: If `part` is not one of 'major', 'minor', or 'patch'.
        """
        if part == "major":
            self.major += 1
            self.minor = 0
            self.patch = 0
        elif part == "minor":
            self.minor += 1
            self.patch = 0
        elif part == "patch":
            self.patch += 1
        else:
            raise ValueError("Invalid part to increment. Valid options: 'major', 'minor', 'patch'.")
