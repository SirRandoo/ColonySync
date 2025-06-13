from argparse import ArgumentParser

__alL__ = ["create_default_parser", "DefaultNamespaceProtocol"]


def create_default_parser() -> ArgumentParser:
    """Returns a new instance of `ArgumentParser` preloaded with common arguments.

    The default instance includes both `--dry-run` and `--level` options.
    """
    parser: ArgumentParser = ArgumentParser(
        description="A collection of scripts for managing the mod."
    )

    parser.add_argument("--dry-run", action="store_true", help="Prints the changes.")
    parser.add_argument(
        "--level", nargs="?", default="INFO", help="The log level to use."
    )

    return parser
