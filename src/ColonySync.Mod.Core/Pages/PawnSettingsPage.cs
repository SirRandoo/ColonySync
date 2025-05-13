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
using ColonySync.Common.Settings;
using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Core.Pages;

/// <summary>Represents the settings page for managing pawn configurations in the mod.</summary>
public class PawnSettingsPage : SettingsPage
{
    private Vector2 _scrollPosition = Vector2.zero;
    private PawnSettings _settings = new();
    private string? _totalOverworkedPawnsBuffer;
    private bool _totalOverworkedPawnsBufferValid;

    public PawnSettingsPage()
    {
        _totalOverworkedPawnsBuffer = _settings.TotalOverworkedPawns.ToString("N0");
        _totalOverworkedPawnsBufferValid = true;
    }

    /// <inheritdoc />
    public override void Draw(Rect region)
    {
        var listing = CreateListing(region);

        listing.Begin(region);
        DrawPawnPoolingSetting(listing);

        if (_settings.Pooling) DrawPawnPoolingRestrictionSetting(listing);

        DrawPawnVacationingSetting(listing);

        if (_settings.Vacationing)
        {
            DrawTotalOverworkedPawnsSetting(listing);
            DrawEmergencyWorkCrisisSetting(listing);
        }

        listing.End();
    }

    private void DrawPawnPoolingSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.PawnPool);
        listing.DrawDescription(KitTranslations.PawnPoolDescription);

        var state = _settings.Pooling;

        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            Pooling = state
        };
    }

    private void DrawPawnPoolingRestrictionSetting(Listing listing)
    {
        var isVip = _settings.PoolingRestrictions.HasFlagFast(UserRoles.Vip);
        var isModerator = _settings.PoolingRestrictions.HasFlagFast(UserRoles.Moderator);
        var isSubscriber = _settings.PoolingRestrictions.HasFlagFast(UserRoles.Subscriber);

        var (labelRegion, fieldRegion) = listing.Split();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);
        LabelDrawer.DrawLabel(labelRegion, KitTranslations.PawnPoolRestrictions);
        listing.DrawDescription(KitTranslations.PawnPoolRestrictionsDescription);

        fieldRegion.height = Text.SmallFontHeight;

        CheckboxDrawer.DrawCheckbox(fieldRegion, KitTranslations.CommonTextVip, ref isVip, TextAnchor.MiddleRight);

        fieldRegion = fieldRegion.Shift(Direction8Way.South, padding: 0f);

        CheckboxDrawer.DrawCheckbox(
            fieldRegion, KitTranslations.CommonTextSubscriber, ref isSubscriber, TextAnchor.MiddleRight
        );

        fieldRegion.Shift(Direction8Way.South, padding: 0f);

        CheckboxDrawer.DrawCheckbox(
            fieldRegion, KitTranslations.CommonTextModerator, ref isModerator, TextAnchor.MiddleRight
        );

        GUI.color = Color.white;

        var @default = UserRoles.None;

        if (isVip) @default |= UserRoles.Vip;

        if (isSubscriber) @default |= UserRoles.Subscriber;

        if (isModerator) @default |= UserRoles.Moderator;

        _settings = _settings with
        {
            PoolingRestrictions = @default
        };
    }

    private void DrawPawnVacationingSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.PawnVacationing);
        listing.DrawDescription(KitTranslations.PawnVacationingDescription);

        var state = _settings.Vacationing;

        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            Vacationing = state
        };
    }

    private void DrawTotalOverworkedPawnsSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.OverworkedPawns);
        listing.DrawDescription(KitTranslations.OverworkedPawnsDescription);

        if (FieldDrawer.DrawNumberField(
                fieldRegion, out var value, ref _totalOverworkedPawnsBuffer,
                ref _totalOverworkedPawnsBufferValid, minimum: 1, short.MaxValue
            ))
            _settings = _settings with
            {
                TotalOverworkedPawns = (short)value
            };
    }

    private void DrawEmergencyWorkCrisisSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();
        IconDrawer.DrawExperimentalIconCutout(ref labelRegion);

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.EmergencyWorkCrisis);
        listing.DrawDescription(KitTranslations.EmergencyWorkCrisisDescription);

        var state = _settings.EmergencyWorkCrisis;

        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            EmergencyWorkCrisis = state
        };
    }
}