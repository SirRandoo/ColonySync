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

using ColonySync.Common.Enums;
using ColonySync.Common.Extensions;
using ColonySync.Common.Settings;
using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Core.Pages;

/// <summary>Represents a settings page for configuring poll-related settings.</summary>
public class PollSettingsPage : SettingsPage
{
    private string? _durationBuffer;
    private bool _durationBufferValid;
    private UnitOfTime _durationTimeUnit;
    private double _lastDurationParseResult;

    private string? _maximumOptionsBuffer;
    private bool _maximumOptionsBufferValid;

    private Vector2 _scrollPosition = Vector2.zero;
    private PollSettings _settings = new();

    public PollSettingsPage()
    {
        _durationTimeUnit = _settings.Duration.GetLongestTimePeriod();
        _durationBuffer = _settings.Duration.ToString(_durationTimeUnit);
        _durationBufferValid = true;

        _maximumOptionsBuffer = _settings.MaximumOptions.ToString("N0");
        _maximumOptionsBufferValid = true;
    }

    /// <inheritdoc />
    public override void Draw(Rect region)
    {
        var listing = CreateListing(region);

        listing.Begin(region);
        DrawDurationSetting(listing);
        DrawMaximumOptionsSetting(listing);
        DrawPreferNativePollsSetting(listing);

        if (!_settings.PreferNativePolls)
        {
            DrawPollDialogSetting(listing);
            DrawOptionsInChatSetting(listing);
        }

        if (_settings is { ShowPollDialog: true, PreferNativePolls: false })
        {
            DrawLargeTextSetting(listing);
            DrawAnimateVotesSetting(listing);
        }

        DrawRandomPollsSetting(listing);

        listing.End();
    }

    private void DrawDurationSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.PollDuration);
        listing.DrawDescription(KitTranslations.PollDurationDescription);

        var dropdownWidth = fieldRegion.width * 0.55f;

        var dropdownRegion = new Rect(
            fieldRegion.x + fieldRegion.width - dropdownWidth, fieldRegion.y, dropdownWidth, fieldRegion.height
        );

        fieldRegion.width = fieldRegion.width - dropdownWidth - 4f;

        if (FieldDrawer.DrawNumberField(
                fieldRegion, out var value, ref _durationBuffer!, ref _durationBufferValid
            ))
        {
            _lastDurationParseResult = value;
            _settings = _settings with
            {
                Duration = _durationTimeUnit.ToTimeSpan(value)
            };
        }

        DropdownDrawer.Draw(
            dropdownRegion, _durationTimeUnit.Name,
            UnitOfTimeNames,
            newUnit =>
            {
                _durationTimeUnit = UnitOfTime.TryParse(newUnit, out var parsedUnit)
                    ? parsedUnit
                    : UnitOfTime.Seconds;

                _settings = _settings with
                {
                    Duration = _durationTimeUnit.ToTimeSpan(_lastDurationParseResult)
                };
            }
        );
    }

    private void DrawMaximumOptionsSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.MaximumPollOptions);
        listing.DrawDescription(KitTranslations.MaximumPollOptionsDescription);

        if (FieldDrawer.DrawNumberField(
                fieldRegion, out var value, ref _maximumOptionsBuffer, ref _maximumOptionsBufferValid,
                minimum: 2
            ))
            _settings = _settings with
            {
                MaximumOptions = value
            };
    }

    private void DrawPreferNativePollsSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.NativePolls);
        listing.DrawDescription(KitTranslations.NativePollsDescription);

        var state = _settings.PreferNativePolls;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            PreferNativePolls = state
        };
    }

    private void DrawPollDialogSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.ShowPollDialog);
        listing.DrawDescription(KitTranslations.ShowPollDialogDescription);

        var state = _settings.ShowPollDialog;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            ShowPollDialog = state
        };
    }

    private void DrawLargeTextSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.LargePollText);

        listing.DrawDescription(KitTranslations.LargePollTextDescription);

        var state = _settings.UseLargeText;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            UseLargeText = state
        };
    }

    private void DrawAnimateVotesSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.AnimatedPollVotes);
        listing.DrawDescription(KitTranslations.AnimatedPollVotesDescription);

        var state = _settings.AnimateVotes;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            AnimateVotes = state
        };
    }

    private void DrawRandomPollsSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.GenerateRandomPolls);
        listing.DrawDescription(KitTranslations.GenerateRandomPollsDescriptions);

        var state = _settings.GenerateRandomPolls;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            GenerateRandomPolls = state
        };
    }

    private void DrawOptionsInChatSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.PollOptionsInChat);
        listing.DrawDescription(KitTranslations.PollOptionsInChatDescription);

        var state = _settings.PostOptionsInChat;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            PostOptionsInChat = state
        };
    }
}