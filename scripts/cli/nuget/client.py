from asyncio import Lock
from logging import getLogger
from logging import Logger
from typing import Final

from aiohttp import ClientSession
from yarl import URL

from .data import SearchQueryDataEntry
from .data import SearchQueryResponse
from .errors import NuGetError
from .errors import PackageQueryTimeoutError
from .errors import ServiceTimeoutError

__all__ = ["NuGetClient"]


class NuGetClient:
    """
    Handles interactions with the NuGet package registry.

    This class provides functionality to query, initialize, and manage
    connections to the NuGet package registry, enabling users to retrieve
    information on available packages. It supports searching for specific
    packages by name and optionally filtering by pre-release versions. The
    class ensures proper handling of session management and error handling
    during the communication with the NuGet API.

    Attributes:
        NUGET_PACKAGE_SOURCE_URL (str):
            The default URL for the NuGet package source.
        SEARCH_QUERY_SERVICE_NAME (str):
            The name of the service used for querying the NuGet package registry.
    """

    NUGET_PACKAGE_SOURCE_URL: Final[str] = "https://api.nuget.org/v3/index.json"
    SEARCH_QUERY_SERVICE_NAME: Final[str] = "SearchQueryService"

    def __init__(
        self,
        nuget_url: str | None = None,
        api_key: str | None = None,
    ):
        self._nuget_url = nuget_url or NuGetClient.NUGET_PACKAGE_SOURCE_URL
        self._api_key = api_key

        self._session = ClientSession()
        self._catalog_lock: Lock = Lock()
        self._logger: Logger = getLogger(__name__)
        self._catalog_query_url: URL | None = None

        if self._api_key:
            self._session.headers.add("X-NuGet-ApiKey", self._api_key)

    async def query_package(
        self, package_name: str, include_prereleases: bool = True
    ) -> list[SearchQueryDataEntry]:
        """
        Queries a package from a NuGet-like source asynchronously.

        This function interacts with a package repository to search for packages matching
        the given name. It supports an optional parameter to include pre-releases in the
        query. The data is returned as a list of package entries. If the query times out,
        an empty list is returned, and the timeout incident is logged.

        Args:
            package_name: Name of the package to search for.
            include_prereleases:
                Flag indicating whether to include pre-release packages in the query.
                Defaults to True.

        Returns:
            A list of package data entries matching the query, or an empty list if the
            query times out.
        """
        try:
            response = await self._query_package(package_name, include_prereleases)
        except PackageQueryTimeoutError as e:
            self._logger.error("Nuget query timed out", exc_info=e)

            return []

        return response.data

    async def _query_package(
        self, package_name: str, include_prereleases: bool = True
    ) -> SearchQueryResponse:
        if not self._catalog_query_url:
            await self._init_catalog_query_url()

        request_url = self._catalog_query_url.with_query(
            {"q": package_name, "prerelease": include_prereleases}
        )

        async with self._session.get(request_url) as response:
            if not response.ok:
                raise PackageQueryTimeoutError()

            return await response.json()

    async def _init_catalog_query_url(self):
        if self._catalog_query_url:
            return

        await self._catalog_lock.acquire()

        if self._catalog_query_url:
            self._catalog_lock.release()

            return

        async with self._session.get(self._nuget_url) as resp:
            if not resp.ok:
                raise ServiceTimeoutError()

            response = await resp.json()
            for resource in response.get("resources", {}):
                resource_type = resource.get("@type", None)

                if resource_type != NuGetClient.SEARCH_QUERY_SERVICE_NAME:
                    continue

                query_id = resource.get("id", None)

                if not query_id:
                    self._catalog_lock.release()

                    raise NuGetError(
                        f"The 'id' property is missing from "
                        f"{NuGetClient.SEARCH_QUERY_SERVICE_NAME}'s service"
                        f"definition."
                    )

                self._catalog_query_url = URL(query_id)
                self._catalog_lock.release()

                return
