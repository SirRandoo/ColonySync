from argparse import ArgumentParser
from logging import basicConfig
from logging import StreamHandler
from pathlib import Path
from sys import stdout

from consolidate import consolidate_assemblies
from deploy import deploy_mod
from duplicate_finder import find_duplicates
from utils.install_locator import locate_game_install

basicConfig(level="INFO", handlers=[StreamHandler(stdout)])


parser = ArgumentParser(
    prog="ColonySync",
    description="A CLI application for managing ColonySync's deployment process.",
    add_help=True,
)

parser.add_argument(
    "action",
    nargs="?",
    help="The action to perform. Valid actions are: deploy, consolidate, find-duplicates",
)
parser.add_argument(
    "--solution-root",
    type=Path,
    default=Path.cwd(),
    help="The path to the directory containing the mod's solution file. Defaults to the current working directory.",
)
parser.add_argument(
    "--game-root",
    type=Path,
    help="The path to the directory containing the game's installation. By default this will use Steam's default installation directory, and extrapolate from there.",
)
parser.add_argument(
    "--dry-run",
    action="store_true",
    help="Prints the changes that would be made.",
)

arguments = parser.parse_args()

match arguments.action:
    case "deploy":
        deploy_mod(arguments.game_root or locate_game_install(), arguments.dry_run)
    case "consolidate":
        consolidate_assemblies(arguments.dry_run)
    case "find-duplicates":
        find_duplicates(arguments.solution_root)
