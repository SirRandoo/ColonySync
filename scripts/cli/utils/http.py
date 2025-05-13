import orjson as json
from urllib.error import URLError, HTTPError
from urllib.request import urlopen

__all__ = ["MicroHttpClient"]


class MicroHttpClient:
    """A minimal HTTP client for perform a GET request and parse JSON responses."""

    @staticmethod
    def get(url: str) -> str:
        """
        Performs an HTTP GET request on the given URL.

        Args:
            url: The URL to send the GET request to.

        Returns:
            The response body as a string.

        Raises:
            URLError: If there is an error with the URL.
            HTTPError: If there is an HTTP issue.
        """
        try:
            with urlopen(url) as response:
                return response.read().decode("utf-8")
        except (URLError, HTTPError) as e:
            raise RuntimeError(f"Failed to fetch URL: {e}")

    @staticmethod
    def fetch_json(url: str) -> dict:
        """
        Fetches the contents of a URL and parses it as JSON.

        Args:
            url: The URL to fetch.

        Returns:
            A dictionary containing the parsed JSON data.

        Raises:
            ValueError: If the response body is not valid JSON.
        """
        content = MicroHttpClient.get(url)
        return json.loads(content)
