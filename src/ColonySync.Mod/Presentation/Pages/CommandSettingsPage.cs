// MIT License
// 
// Copyright (c) 2025 sirrandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using ColonySync.Mod.Settings;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Pages;

/// <summary>Represents the settings page for configuring command-related settings in the application.</summary>
public class CommandSettingsPage : SettingsPage
{
    private Vector2 _scrollPosition = Vector2.zero;
    private CommandSettings _settings = new();

    /// <inheritdoc />
    public override void Draw(Rect region)
    {
        var listing = CreateListing(region);

        listing.Begin(region);
        DrawCommandPrefixSetting(listing);
        DrawCommandEmojisSetting(listing);
        listing.End();
    }

    private void DrawCommandPrefixSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.CommandPrefix);
        listing.DrawDescription(KitTranslations.CommandPrefixDescription);

        if (FieldDrawer.DrawTextField(fieldRegion, _settings.Prefix, out var newPrefix))
            _settings = _settings with
            {
                Prefix = newPrefix
            };
    }

    private void DrawCommandEmojisSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.CommandEmojis);
        listing.DrawDescription(KitTranslations.CommandEmojisDescription);

        var state = _settings.UseEmojis;

        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            UseEmojis = state
        };
    }
}