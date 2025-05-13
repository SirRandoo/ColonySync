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

using ColonySync.Common.Settings;
using ColonySync.Mod.Presentation.Drawers;
using ColonySync.Mod.Presentation.Extensions;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Core.Pages;

/// <summary>Represents the settings page for store settings in the application.</summary>
public class StoreSettingsPage : SettingsPage
{
    private string? _minimumPurchasePriceBuffer;
    private bool _minimumPurchasePriceBufferValid;
    private Vector2 _scrollPosition = Vector2.zero;
    private StoreSettings _settings = new();

    private string? _storeLinkBuffer;
    private bool _storeLinkBufferValid;

    public StoreSettingsPage()
    {
        _storeLinkBuffer = _settings.StoreLink.ToStringNullable();
        _storeLinkBufferValid = !string.IsNullOrEmpty(_storeLinkBuffer);

        _minimumPurchasePriceBuffer = _settings.MinimumPurchasePrice.ToString("N0");
        _minimumPurchasePriceBufferValid = true;
    }

    /// <inheritdoc />
    public override void Draw(Rect region)
    {
        var listing = CreateListing(region);

        listing.Begin(region);
        DrawStoreLinkSetting(listing);
        DrawMinimumPurchasePriceSetting(listing);
        DrawPurchaseConfirmationSetting(listing);
        DrawBuildingsPurchasableSetting(listing);
        listing.End();
    }

    private void DrawStoreLinkSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.StoreLink);
        listing.DrawDescription(KitTranslations.StoreLinkDescription);

        if (FieldDrawer.DrawUri(fieldRegion, out var value, ref _storeLinkBuffer, ref _storeLinkBufferValid))
            _settings = _settings with
            {
                StoreLink = value
            };
    }

    private void DrawMinimumPurchasePriceSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.MinimumPurchasePrice);
        listing.DrawDescription(KitTranslations.MinimumPurchasePriceDescription);

        if (FieldDrawer.DrawNumberField(
                fieldRegion, out int value, ref _minimumPurchasePriceBuffer,
                ref _minimumPurchasePriceBufferValid
            ))
            _settings = _settings with
            {
                MinimumPurchasePrice = value
            };
    }

    private void DrawPurchaseConfirmationSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.PurchaseConfirmations);
        listing.DrawDescription(KitTranslations.PurchaseConfirmationsDescription);

        var state = _settings.PurchaseConfirmations;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            PurchaseConfirmations = state
        };
    }

    private void DrawBuildingsPurchasableSetting(Listing listing)
    {
        var (labelRegion, fieldRegion) = listing.Split();
        fieldRegion = fieldRegion.TrimToIconRect();

        LabelDrawer.DrawLabel(labelRegion, KitTranslations.PurchaseBuildings);
        listing.DrawDescription(KitTranslations.PurchaseBuildingsDescription);

        var state = _settings.BuildingsPurchasable;
        CheckboxDrawer.DrawCheckbox(fieldRegion, ref state);
        _settings = _settings with
        {
            BuildingsPurchasable = state
        };
    }
}