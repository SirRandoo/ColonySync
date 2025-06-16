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

using ColonySync.Common;
using ColonySync.Common.Settings;
using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Presentation.Pages;

/// <summary>Represents the morality settings page in the settings menu.</summary>
public class MoralitySettingsPage : SettingsPage
{
    private string? _karmaRangeMaximumBuffer;
    private bool _karmaRangeMaximumBufferValid;
    private string? _karmaRangeMinimumBuffer;
    private bool _karmaRangeMinimumBufferValid;
    private Vector2 _scrollPosition = Vector2.zero;
    private MoralitySettings _settings = new();
    private string? _startingKarmaBuffer;
    private bool _startingKarmaBufferValid;

    public MoralitySettingsPage()
    {
        _startingKarmaBuffer = _settings.StartingAlignment.ToString("N0");
        _startingKarmaBufferValid = true;

        _karmaRangeMinimumBuffer = _settings.AlignmentRange.Minimum.ToString("N0");
        _karmaRangeMinimumBufferValid = true;

        _karmaRangeMaximumBuffer = _settings.AlignmentRange.Maximum.ToString("N0");
        _karmaRangeMaximumBufferValid = true;
    }

    /// <inheritdoc />
    public override void Draw(Rect region)
    {
        var listing = CreateListing(region);

        listing.Begin(region);
        DrawKarmaEnabledSetting(listing);

        if (_settings is { IsAlignmentEnabled: true, AlignmentRange: not null })
        {
            DrawStartingKarmaSetting(listing);
            DrawKarmaRangeSetting(listing);
        }

        DrawReputationEnabledSetting(listing);
        listing.End();
    }

    private void DrawKarmaEnabledSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.KarmaSystem);
        listing.DrawDescription(KitTranslations.KarmaSystemDescription);

        var state = _settings.IsAlignmentEnabled;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            IsAlignmentEnabled = state
        };
    }

    private void DrawStartingKarmaSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.StartingKarma);
        listing.DrawDescription(KitTranslations.StartingKarmaDescription);

        if (FieldDrawer.DrawNumberField(
                fieldRegion, out var value, ref _startingKarmaBuffer, ref _startingKarmaBufferValid,
                short.MinValue, short.MaxValue
            ))
            _settings = _settings with
            {
                StartingAlignment = (short)value
            };
    }

    private void DrawKarmaRangeSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        LabelDrawer.DrawLabel(labelRegion, KitTranslations.KarmaRange);
        listing.DrawDescription(KitTranslations.KarmaRangeTooltip);

        var halvedWidth = fieldRegion.width * 0.5f;

        var minimumRegion = new Rect(
            fieldRegion.x, fieldRegion.y, halvedWidth - Text.SmallFontHeight * 0.5f, fieldRegion.height
        );

        var maximumRegion = new Rect(
            minimumRegion.x + minimumRegion.width + Text.SmallFontHeight, minimumRegion.y, minimumRegion.width,
            minimumRegion.height
        );

        var rangeIconRegion = RectExtensions.IconRect(
            maximumRegion.x - Text.SmallFontHeight, maximumRegion.y, Text.SmallFontHeight, Text.SmallFontHeight
        );

        LabelDrawer.DrawLabel(
            rangeIconRegion, text: "~", DescriptionDrawer.DescriptionTextColor, TextAnchor.LowerCenter
        );

        if (FieldDrawer.DrawNumberField(
                minimumRegion, out var newMinimum, ref _karmaRangeMinimumBuffer,
                ref _karmaRangeMinimumBufferValid, short.MinValue, short.MaxValue
            ))
            _settings = _settings with
            {
                AlignmentRange = new ShortRange((short)newMinimum, _settings.AlignmentRange!.Maximum)
            };

        if (FieldDrawer.DrawNumberField(
                maximumRegion, out var newMaximum, ref _karmaRangeMaximumBuffer,
                ref _karmaRangeMaximumBufferValid, short.MinValue, short.MaxValue
            ))
            _settings = _settings with
            {
                AlignmentRange = new ShortRange(_settings.AlignmentRange!.Minimum, (short)newMaximum)
            };
    }

    private void DrawReputationEnabledSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.ReputationSystem);
        listing.DrawDescription(KitTranslations.ReputationSystemDescription);

        var state = _settings.IsReputationEnabled;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            IsReputationEnabled = state
        };
    }
}
