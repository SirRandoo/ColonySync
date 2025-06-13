from logging import basicConfig
from logging import StreamHandler
from logging import getLevelNamesMapping
from typing import Literal

__all__ = ["init_logging"]

type _LogLevel = (
    Literal["INFO"]
    | Literal["DEBUG"]
    | Literal["WARNING"]
    | Literal["ERROR"]
    | Literal["CRITICAL"]
    | Literal["FATAL"]
    | Literal["WARN"]
)


def init_logging(level: _LogLevel | None = None) -> None:
    """Initializes the logging system with standardized formatting.

    Args:
        level: The log level to use. Defaults to "INFO".
    Raises:
        ValueError: The log level given was not recognized.
    """
    if level is None:
        level = "INFO"
    elif level not in getLevelNamesMapping():
        raise ValueError(f"Level '{level}' is not a recognized logging level")

    basicConfig(
        level=level,
        format="%(asctime)s %(levelname)s %(name)s %(message)s",
        handlers=[StreamHandler()],
    )
