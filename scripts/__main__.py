import shutil
from pathlib import Path
from platform import system
from typing import Final

import click

from about import get_mod_package_id
from corpus import load_corpus
from environment import Environment
from mods_config import load_mods_config
from mods_config import save_mods_config
from cli.utils.click import (
    echo_debug,
    echo_error,
    echo_exception,
    echo_info,
    echo_warning,
)

HARMONY_MOD_ID: Final[str] = "brrainz.harmony"
KNOWN_LIBRARIES: Final[set[str]] = set()
PROVIDED_ASSEMBLIES: Final[set[str]] = {
    "0Harmony",
}
FILTERED_ASSEMBLIES: Final[set[str]] = {
    "NetEscapades.EnumGenerators.Attributes",
}


@click.group()
def main():
    """
    The main entry point for the Click command-line interface (CLI) application.

    This function defines the main entry point for the CLI application and
    registers commands to the CLI group.
    """


@main.command("unnest")
@click.option(
    "dry_run",
    "--dry-run",
    is_flag=True,
    help="Prints the changes without making any changes.",
)
def unnest(dry_run: bool = False) -> None:
    """
    Scans the "Releases" directory for framework directories within game version folders,
    copies contents from the framework directory (net48) to the parent assemblies directory,
    and removes the framework directory after copying.

    Raises:
        IOError: If there is an error, copying or removing directories.
    """
    releases_path: Path = Path("Releases")

    if not releases_path.exists():
        echo_error("Releases directory doesn't exist.")

        return

    echo_info("Scanning releases folder for framework folders...")
    for category in releases_path.iterdir():
        for game_version in category.iterdir():
            assemblies_directory: Path = game_version.joinpath("Assemblies")
            framework_directory: Path = assemblies_directory.joinpath("net48")

            if framework_directory.exists():
                echo_info(f"Found framework directory {framework_directory}")
                click.secho(
                    f"  Copying to {assemblies_directory} ...",
                    fg="green",
                    nl=False,
                )

                if not dry_run:
                    shutil.copytree(
                        framework_directory, assemblies_directory, dirs_exist_ok=True
                    )

                echo_info("Done!")

                click.secho(
                    f"  Removing framework directory....",
                    fg="green",
                    nl=False,
                )

                if not dry_run:
                    shutil.rmtree(framework_directory)

                echo_info("Done!")

    echo_info("Done!")


@main.command("condense")
@click.option("dry_run", "--dry-run", is_flag=True, help="Prints the changes.")
def condense(dry_run: bool = False) -> None:
    """
    Condenses and organizes assemblies and resources by eliminating duplicates and moving them to their respective common directories.

    The function executes the following steps:
    1. Loads the corpus from 'Corpus.xml'.
    2. Maps common resources and native resources to their paths.
    3. Scans the 'Releases' directory to identify and handle duplicate assemblies.
    4. Removes or relocates common libraries to avoid redundancy.
    5. Ensures '.pdb' files associated with assemblies are correctly managed.
    6. Handles common native resources in the 'dll', 'so', and 'dylib' formats, ensuring they are moved or deleted as required.

    This ensures a clean and organized structure for managing assemblies and resources consistently across different releases.
    """

    click.secho("Loading corpus...", nl=False, fg="green")
    corpus = load_corpus(Path("Corpus.xml"))
    echo_info("Done!")

    common_resources: dict[str, Path] = {}
    common_native_resources: dict[str, Path] = {}
    common_natives_path: Path = Path("Common/Natives/Assemblies")
    common_libraries_path: Path = Path("Common/Libraries/Assemblies")

    if not common_libraries_path.exists():
        click.secho(
            "Common libraries directory doesn't exist. Creating...",
            fg="yellow",
            nl=False,
        )

        if not dry_run:
            common_libraries_path.mkdir(parents=True, exist_ok=True)

        echo_warning("Done!")

    if not common_natives_path.exists():
        click.secho(
            "Common natives directory doesn't exist. Creating...", nl=False, fg="yellow"
        )

        if not dry_run:
            common_natives_path.mkdir(parents=True, exist_ok=True)

        echo_warning("Done!")

    click.secho("Mapping common assemblies...", nl=False, fg="green")
    for bundle in corpus.bundles:
        if (
            bundle.root.joinpath("Assemblies").resolve()
            == common_libraries_path.resolve()
        ):
            for resource in bundle.resources:
                resource_path = Path(bundle.root)

                if resource.root:
                    resource_path.joinpath(resource.root)

                common_resources[resource.name] = resource_path.joinpath(resource.name)
        elif (
            bundle.root.joinpath("Assemblies").resolve()
            == common_natives_path.resolve()
        ):
            for resource in bundle.resources:
                resource_path = Path(bundle.root)

                if resource.root:
                    resource_path.joinpath(resource.root)

                common_native_resources[resource.name] = resource_path.joinpath(
                    resource.name
                )

    echo_info("Done!")

    echo_info("Scanning assemblies in './Releases/' ...")

    releases_path: Path = Path("Releases")

    if not releases_path.exists():
        echo_error("Releases directory doesn't exist.")

        return

    for category in releases_path.iterdir():
        if category.name.casefold() == "bootstrap":
            continue

        for game_version in category.iterdir():
            for assembly in game_version.joinpath("Assemblies").iterdir():
                stem: str = assembly.stem

                if assembly.suffix == ".pdb":
                    continue

                if stem in PROVIDED_ASSEMBLIES:
                    echo_info("Found provided assembly; deleting...")

                    if not dry_run:
                        assembly.unlink(missing_ok=True)
                        assembly.with_suffix(".pdb").unlink(missing_ok=True)

                if stem in FILTERED_ASSEMBLIES:
                    echo_info("Found filtered assembly; deleting...")

                    if not dry_run:
                        assembly.unlink(missing_ok=True)
                        assembly.with_suffix(".pdb").unlink(missing_ok=True)

                if stem.casefold().startswith("ColonySync.Mod.Shared".casefold()):
                    echo_info("Found shared assembly; deleting...")

                    if not dry_run:
                        assembly.unlink(missing_ok=True)
                        assembly.with_suffix(".pdb").unlink(missing_ok=True)

                if stem in KNOWN_LIBRARIES:
                    echo_info(
                        f"  Located common library {assembly.name} in {assembly.parent}"
                    )

                    if common_libraries_path.joinpath(assembly.name).exists():
                        click.secho(
                            "    Removing potentially stale binary...",
                            nl=False,
                            fg="green",
                        )

                        if not dry_run:
                            common_libraries_path.joinpath(assembly.name).unlink(
                                missing_ok=True
                            )

                        echo_info("Done!")

                    click.secho(
                        f"    Moving to common directory {common_libraries_path} ...",
                        nl=False,
                        fg="green",
                    )

                    if not dry_run:
                        shutil.move(assembly, common_libraries_path)

                    echo_info("Done!")

                    pdb_file = assembly.with_suffix(".pdb")

                    if pdb_file.exists():
                        if common_libraries_path.joinpath(pdb_file.name).exists():
                            click.secho(
                                "    Deleting potentially stale pdb file...",
                                nl=False,
                                fg="green",
                            )

                            if not dry_run:
                                common_libraries_path.joinpath(pdb_file.name).unlink(
                                    missing_ok=True
                                )

                            echo_info("Done!")

                        click.secho(
                            f"    Moving pdb file for {assembly.name} ...",
                            nl=False,
                            fg="green",
                        )

                        if not dry_run:
                            shutil.move(pdb_file, common_libraries_path)

                        echo_info("Done!")

                    continue

                if stem in common_resources:
                    echo_info(
                        f"  Located common assembly {assembly.name} in {assembly.parent}"
                    )

                    if not common_libraries_path.joinpath(assembly.name).exists():
                        click.secho(
                            f"    Moving to common directory {common_libraries_path} ...",
                            nl=False,
                            fg="green",
                        )

                        if not dry_run:
                            shutil.move(assembly, common_libraries_path)

                        echo_info("Done!")
                    else:
                        click.secho(
                            "    Deleting duplicate assembly...", nl=False, fg="green"
                        )

                        if not dry_run:
                            assembly.unlink(missing_ok=True)

                        echo_info("Done!")

                    pdb_file = assembly.with_suffix(".pdb")

                    if pdb_file.exists():
                        if not common_libraries_path.joinpath(pdb_file.name).exists():
                            click.secho(
                                f"   Moving pdb file for {assembly.name} ...",
                                nl=False,
                                fg="green",
                            )

                            if not dry_run:
                                shutil.move(pdb_file, common_libraries_path)

                            echo_info("Done!")
                        else:
                            click.secho(
                                "    Deleted duplicate pdb file...",
                                nl=False,
                                fg="green",
                            )

                            if not dry_run:
                                pdb_file.unlink()

                            echo_info("Done!")

                elif stem in common_native_resources and assembly.suffix in {
                    ".dll",
                    ".so",
                    ".dylib",
                }:
                    echo_info(f"  Located common native file {assembly.stem}")

                    if not common_natives_path.joinpath(
                        f"{assembly.stem}.dll"
                    ).exists():
                        click.secho(
                            f"   Moving {assembly.stem}.dll ...", nl=False, fg="green"
                        )

                        if not dry_run:
                            shutil.move(
                                assembly.with_suffix(".dll"), common_natives_path
                            )

                        echo_info("Done!")
                    else:
                        click.secho(
                            "    Deleting duplicate native file...",
                            nl=False,
                            fg="green",
                        )

                        if not dry_run:
                            assembly.with_suffix(".dll").unlink(missing_ok=True)

                        echo_info("Done!")

                    if not common_natives_path.joinpath(f"{assembly.stem}.so").exists():
                        click.secho(
                            f"   Moving {assembly.stem}.so ...", nl=False, fg="green"
                        )

                        if not dry_run:
                            shutil.move(
                                assembly.with_suffix(".so"), common_natives_path
                            )

                        echo_info("Done!")
                    else:
                        click.secho(
                            "    Deleted duplicate native file...", nl=False, fg="green"
                        )

                        if not dry_run:
                            assembly.with_suffix(".so").unlink(missing_ok=True)

                        echo_info("Done!")

                    if not common_natives_path.joinpath(
                        f"{assembly.stem}.dylib"
                    ).exists():
                        click.secho(
                            f"   Moving {assembly.stem}.dylib ...", nl=False, fg="green"
                        )

                        if not dry_run:
                            shutil.move(
                                assembly.with_suffix(".dylib"), common_natives_path
                            )

                        echo_info("Done!")
                    else:
                        click.secho(
                            "    Deleted duplicate native file...", nl=False, fg="green"
                        )

                        if not dry_run:
                            assembly.with_suffix(".dylib").unlink(missing_ok=True)

                        echo_info("Done!")

    echo_info("Deduplicated assemblies in './Releases/'")


@main.command("deploy")
@click.option("dry_run", "--dry-run", is_flag=True, help="Prints the changes.")
def deploy(dry_run: bool = False) -> None:
    """
    Deploys the ColonySync mod to the RimWorld game directory by performing a series of file operations.

    Steps include:
    1. Locating the game install directory.
    2. Copying the necessary files and directories to the mod directory.
    3. Deleting existing directories before copying new ones if they exist.

    Raises:
        FileNotFoundError: If any of the required files or directories aren’t found in the source location.
        PermissionError: If there aren’t enough permissions to read/write the necessary files.
    """
    echo_info("Deploying ColonySync...")

    click.secho("Locating game install location...", nl=False, fg="green")
    env = Environment.create_instance(dry_run=dry_run)
    echo_info("Done!")

    mod_directory = env.game_install_path.joinpath("Mods", "ColonySync")
    echo_info(f"RimWorld is installed @ {env.game_install_path}")

    click.secho("Copying corpus file to mod directory...", nl=False, fg="green")
    if not dry_run:
        shutil.copy(Path("Corpus.xml"), mod_directory)
    echo_info("Done!")

    click.secho("Copying load folders file to mod directory...", nl=False, fg="green")
    if not dry_run:
        shutil.copy(Path("LoadFolders.xml"), mod_directory)
    echo_info("Done!")

    click.secho("Copying README file to mod directory...", nl=False, fg="green")
    if not dry_run:
        shutil.copy(Path("README.md"), mod_directory)
    echo_info("Done!")

    click.secho("Copying LICENSE file to mod directory...", nl=False, fg="green")
    if not dry_run:
        shutil.copy(Path("LICENSE"), mod_directory)
    echo_info("Done!")

    click.secho("Copying About directory to mod directory...", nl=False, fg="green")
    if not dry_run:
        shutil.copytree(
            Path("About"), mod_directory.joinpath("About"), dirs_exist_ok=True
        )
    echo_info("Done!")

    if mod_directory.joinpath("Releases").exists():
        click.secho(
            "Deleting Releases directory in mod directory...", nl=False, fg="green"
        )
        if not dry_run:
            shutil.rmtree(mod_directory.joinpath("Releases"))
        echo_info("Done!")

    click.secho("Copying Releases directory to mod directory...", nl=False, fg="green")
    if not dry_run:
        shutil.copytree(
            Path("Releases"), mod_directory.joinpath("Releases"), dirs_exist_ok=True
        )
    echo_info("Done!")

    if mod_directory.joinpath("Common").exists():
        click.secho(
            "Deleting Common directory in mod directory...", nl=False, fg="green"
        )
        if not dry_run:
            shutil.rmtree(mod_directory.joinpath("Common"))
        echo_info("Done!")

    click.secho("Copying Common directory to mod directory...", nl=False, fg="green")
    if not dry_run:
        shutil.copytree(
            Path("Common"), mod_directory.joinpath("Common"), dirs_exist_ok=True
        )
    echo_info("Done!")


@main.command("ensure-active")
@click.option(
    "dry_run",
    "--dry-run",
    is_flag=True,
    help="Prints the changes without making any changes.",
)
def update_mod_list(dry_run: bool = False) -> None:
    """
    Handles the updating of the mod list by ensuring specific mods are active.

    Discovers the save data folder based on the operating system and checks for
    the existence of specific mods in the mod configuration file.
    - For Windows, constructs the path based on typical AppData location.
    - For macOS, constructs the path based on the typical Library location with a fallback path.
    - For Linux, prints a message that saves data discovery is not implemented.

    Loads the mods configuration file and ensures the presence of required mods
    (HARMONY_MOD_ID and UX_MOD_ID) in the active mods list. If changes are made
    to the configuration, the updated configuration is saved.

    Raises:
        ValueError: If the mods configuration file couldn’t be loaded.

    Prints:
        A message indicating the discovered save data folder path for the platform.
        Error message if the mod configuration file can't be loaded.
    """
    save_data_folder: Path = Path.home()

    match system().casefold():
        case "windows":
            save_data_folder = save_data_folder.joinpath(
                "AppData", "LocalLow", "Ludeon Studios", "RimWorld by Ludeon Studios"
            )
        case "darwin":  # Discovery on Mac is probably wrong.
            click.echo("Save data discovery on MacOS may not be implemented properly.")

            save_data_folder = save_data_folder.joinpath(
                "Library",
                "Application Support",
                "Ludeon Studios",
                "RimWorld by Ludeon Studios",
            )

            if not save_data_folder.exists():
                save_data_folder = Path.home().joinpath(
                    "Library",
                    "Application Support",
                    "unity.ludeon studios.rimworld by ludeon studios",
                )
        case "linux":
            save_data_folder = Path.home().joinpath(
                ".config", "unity3d", "Ludeon Studios", "RimWorld by Ludeon Studios"
            )

    echo_info(
        f"Discovered save data folder @ {save_data_folder} for platform '{system()}'"
    )

    mods_config_file_path: Path = save_data_folder.joinpath("Config", "ModsConfig.xml")

    echo_info(f"Loading mods config @ {mods_config_file_path}...")

    try:
        config = load_mods_config(mods_config_file_path)
    except ValueError as e:
        echo_error("Could not load mods config; aborting...")
        echo_exception(e)

        raise e
    else:
        echo_info("Loaded mods config.")
        echo_info("Checking for required mods in active mods list...")

        changed: bool = False

        click.secho("Fetching package id from About.xml...", nl=False, fg="green")
        mod_id: str | None = get_mod_package_id(Path("About/About.xml"))
        echo_info("Done!")

        if mod_id is None:
            echo_error("Could not determine mod package id from About.xml; aborting...")

            return

        echo_info("Checking if mod is active...")
        if not mod_id in config.active_mods:
            click.secho("Mod is not active; adding...", nl=False, fg="green")
            config.active_mods.append(mod_id)
            echo_info("Done!")

            changed = True

        echo_info("Checking if Harmony is active...")
        if not HARMONY_MOD_ID in config.active_mods:
            click.secho("Harmony is not active; adding...", nl=False, fg="green")
            config.active_mods.insert(0, HARMONY_MOD_ID)
            echo_info("Done!")

            changed = True

        if changed:
            click.secho("Changes detected; saving...", nl=False, fg="green")
            save_mods_config(mods_config_file_path, config, dry_run=dry_run)
            echo_info("Done!")


if __name__ == "__main__":
    main()
