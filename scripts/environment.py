"""
This file contains the `Environment` class, which indirectly indexes the file
system for the Steam install directory. Once the directory is indexed for the
first time, it's saved to disk for quicker access.
"""

from dataclasses import dataclass
from dataclasses import field
from pathlib import Path
from typing import Self

from probe import locate_steam_install


@dataclass(slots=True)
class Environment:
    """
    Represents the environment configuration for locating game install paths.

    Attributes:
        steam_install_path: The installation path of Steam. Defaults to the result of locate_steam_install().

    Methods:
        game_install_path: Returns the location of the game's installation path.
        game_workshop_path: Returns the location of the game's workshop installation path.
        create_instance: Creates a new instance of the `Environment` class. Raises a ValueError if the Steam installation path could not be found.
    """

    steam_install_path: Path | None = field(default_factory=locate_steam_install)

    @property
    def game_install_path(self):
        """
        @property
        def game_install_path(self):
            Retrieves the full directory path where the game RimWorld is installed.

            This property constructs and returns the path by appending the specific
            subdirectory "\common\RimWorld" to the base Steam installation path
            stored in `self.steam_install_path`.

            Returns:
                Path: The full path to the RimWorld game directory.
        """
        return self.steam_install_path.joinpath("common\\RimWorld")

    @property
    def game_workshop_path(self):
        """
        Return the path to the workshop directory for the game.

        The path constructed is based on the Steam installation directory
        combined with the relative path to the game's workshop directory.

        Returns:
            Path: The path to the workshop directory for the game.
        """
        return self.steam_install_path.joinpath("workshop\\294100")

    @classmethod
    def create_instance(cls, dry_run: bool = False) -> Self:
        """
        Creates an instance of the class. If a steam installation path file exists, reads the path from the file
        and initializes the class with it. If the file does not exist, creates a new instance and writes the
        steam installation path to the file if it is set.

        Returns:
            Self: An instance of the class.

        Raises:
            ValueError: If the steam installation path is not set after creating a new instance.
        """
        steam_path_file: Path = Path(".run\\.steam")

        if steam_path_file.exists():
            with steam_path_file.open() as f:
                path = Path(f.read())

                return cls(path)

        instance: Self = cls()

        if instance.steam_install_path is None:
            raise ValueError("Steam installation path is not set")

        if instance.steam_install_path is not None and not dry_run:
            with steam_path_file.open("w") as f:
                f.write(str(instance.steam_install_path))

        return instance
