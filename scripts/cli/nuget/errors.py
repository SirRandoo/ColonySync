__all__ = [
    "NuGetError",
    "NuGetTimeoutError",
    "ServiceTimeoutError",
    "PackageQueryTimeoutError",
]


class NuGetError(Exception):
    """Custom exception class for NuGet-related errors.

    This exception is designed to handle and represent errors specific
    to NuGet operations. It can be used to provide contextual error
    information when interacting with NuGet-related functionalities or
    APIs.
    """


class NuGetTimeoutError(NuGetError):
    """Exception raised for timeout errors specific to NuGet operations.

    This class represents an error that occurs when a NuGet operation exceeds
    the permitted time limit, indicating a timeout situation. It specifically
    extends the functionality of the base NuGetError exception.
    """


class ServiceTimeoutError(NuGetTimeoutError):
    """
    Exception raised when a service times out.

    The `ServiceTimeoutError` class is a specialized exception triggered
    when a service-specific timeout occurs while using NuGet-related operations. This
    exception extends the base `NuGetTimeoutError` exception, maintaining context
    related to service timeouts.
    """


class PackageQueryTimeoutError(NuGetTimeoutError):
    """
    Custom exception for query timeout errors when interacting with the NuGet API.

    This class represents a specific type of timeout error that occurs during
    a package query operation using NuGet. It inherits from NuGetTimeoutError
    and is intended to provide more granular error handling for scenarios
    where query requests exceed the allowed time limit. This error can be
    used to distinguish query-specific timeouts from other timeout
    situations during NuGet interactions.
    """
