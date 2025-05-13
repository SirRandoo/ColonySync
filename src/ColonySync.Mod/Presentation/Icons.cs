using System.Collections.Generic;
using JetBrains.Annotations;
using NetEscapades.EnumGenerators;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation;

/// <summary>
///     Represents categories of icons that can be used for different visual styles or purposes within the application.
/// </summary>
[EnumExtensions]
public enum IconCategory
{
    Solid,
    Brands,
    Regular
}

/// <summary>
///     Provides functionality for retrieving and managing icons within the application.
/// </summary>
[PublicAPI]
public static class Icons
{
    private static readonly Dictionary<string, Texture2D> IconCache = new();

    /// <summary>
    ///     Retrieves an icon as a <see cref="Texture2D" /> from the specified category and name.
    ///     If the icon is not found, a placeholder icon is returned, and an error is logged.
    /// </summary>
    /// <param name="category">The category of the icon to retrieve.</param>
    /// <param name="icon">The name of the icon to retrieve.</param>
    /// <returns>
    ///     A <see cref="Texture2D" /> representing the requested icon, or a placeholder icon if the requested icon is not
    ///     found.
    /// </returns>
    public static Texture2D Get(IconCategory category, string icon)
    {
        var itemPath = $"Icons/{category.ToStringFast()}/{icon}";

        if (IconCache.TryGetValue(itemPath, out var iconTexture)) return iconTexture;

        iconTexture = ContentFinder<Texture2D>.Get(itemPath);

        if (!iconTexture)
        {
            Log.Error($"Failed to find icon {icon} in category {category}");

            IconCache.Add(itemPath, Widgets.PlaceholderIconTex);

            return Widgets.PlaceholderIconTex;
        }

        IconCache.Add(itemPath, iconTexture);

        return iconTexture;
    }
}
