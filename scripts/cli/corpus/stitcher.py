from .package import Package
from ..utils.http import MicroHttpClient

type NuGetUrl = str
type PackagePattern = str


def stitch(
    packages: list[Package], configs: dict[NuGetUrl, list[PackagePattern]]
) -> None:
    cache: dict[str, Package] = {}
    pattern_map: dict[PackagePattern, NuGetUrl] = {}
    nuget_search_providers: dict[NuGetUrl, NuGetUrl] = {}

    for nuget_url in configs:
        response = MicroHttpClient.fetch_json(nuget_url)
        (search_provider_url, *_) = [
            resource
            for resource in response.get("resources", [])
            if resource.get("@type", "") == "SearchQueryService"
        ]

        nuget_search_providers[nuget_url] = search_provider_url

    for nuget_url, patterns in configs.items():
        for pattern in patterns:
            pattern_map[pattern] = nuget_search_providers[nuget_url]

    for package in packages:
        (search_url, *_) = [
            pattern
            for pattern in pattern_map
            if package.name.startswith(pattern.rstrip("*"))
        ]

        response = MicroHttpClient.fetch_json(
            search_url + f"?q={package.name}&prerelease=true"
        )

        for entry in response.get("data", []):
            if entry.get("id", None) != package.name:
                continue

            (version, *_) = [
                v
                for v in entry.get("versions", [])
                if v.get("version", None) == package.version
            ]

            if not version:
                continue

            version_metadata = MicroHttpClient.fetch_json(version["@id"])
            catalog_entry_url = version_metadata["catalogEntry"]
            catalog_entry_metadata = MicroHttpClient.fetch_json(catalog_entry_url)
            dependency_groups: list[dict] = [
                group
                for group in catalog_entry_metadata.get("dependencyGroups", [])
                if group.get("targetFramework", "").startswith(".NETStandard2.0")
                or group.get("targetFramework", "").startswith(".NETFramework")
            ]

            (preferred_dependency_group, *_) = [
                group
                for group in dependency_groups
                if group.get("targetFramework", "").startswith(".NETFramework")
            ]

            if not preferred_dependency_group:
                preferred_dependency_group = dependency_groups[0]

            for dependency in preferred_dependency_group:
                dependency_package: Package

                if dependency["id"] in cache:
                    dependency_package = cache[dependency["id"]]
                else:
                    dependency_package = Package(
                        dependency["id"],
                        dependency["version"],
                    )

                    cache[dependency["id"]] = dependency_package

                package.dependencies.append(dependency_package)

            package.file_names.update(
                [
                    file["name"]
                    for file in catalog_entry_metadata.get("packageEntries", [])
                ]
            )
