from click import secho
from functools import partial

__all__ = ["echo_info", "echo_warning", "echo_error", "echo_debug", "echo_exception"]

echo_info = partial(secho, fg="green")
echo_warning = partial(secho, fg="yellow")
echo_error = partial(secho, fg="red")
echo_debug = partial(secho, fg="magenta")
echo_exception = partial(secho, fg="red", err=True)
