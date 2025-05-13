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

using System;
using System.Collections.Generic;
using ColonySync.Mod.Presentation.Drawers;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Core.Windows;

/// <summary>
///     Represents a window for proxy settings within the application. Inherits from the Unity `Window` class to
///     provide a customized interface for proxy configurations.
/// </summary>
internal class ProxySettingsWindow : Window
{
    private readonly Verse.Mod _mod;
    private bool _hasSettings;
    private string _lastException = null!;
    private FloatMenu _noSettingsFloatMenu = null!;
    private string _selectModText = null!;
    private bool _settingsCloseAttempted;
    private FloatMenu _settingsFloatMenu = null!;
    private int _totalErrors;

    protected ProxySettingsWindow(Verse.Mod mod)
    {
        _mod = mod;
        forcePause = true;
        doCloseX = true;
        doCloseButton = true;
        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
    }

    public override Vector2 InitialSize => new(x: 900f, y: 700f);

    /// <inheritdoc />
    public override void DoWindowContents(Rect inRect)
    {
        var headerRect = new Rect(x: 0f, y: 0f, inRect.width, height: 35f);
        var settingsRect = new Rect(x: 0f, y: 40f, inRect.width, inRect.height - 40f - CloseButSize.y);

        GUI.BeginGroup(inRect);

        GUI.BeginGroup(headerRect);
        DrawHeader(headerRect);
        GUI.EndGroup();

        GUI.BeginGroup(settingsRect);
        DrawSettings(settingsRect.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawHeader(Rect region)
    {
        var btnRect = new Rect(x: 0f, y: 0f, width: 150f, region.height);
        var labelRect = new Rect(x: 167f, y: 0f, region.width - 150f - 17f, region.height);

        if (Widgets.ButtonText(btnRect, _selectModText))
            Find.WindowStack.Add(_hasSettings ? _settingsFloatMenu : _noSettingsFloatMenu);

        Text.Font = GameFont.Medium;
        Widgets.Label(labelRect, _mod.SettingsCategory());
        Text.Font = GameFont.Small;
    }

    /// <summary>Draws the settings interface within the specified region.</summary>
    /// <param name="region">The rectangular area where the settings should be drawn.</param>
    protected virtual void DrawSettings(Rect region)
    {
        if (_totalErrors >= 20)
        {
            LabelDrawer.DrawLabel(region, _lastException, Color.gray, TextAnchor.UpperLeft);

            return;
        }

        try
        {
            _mod.DoSettingsWindowContents(region);
        }
        catch (Exception e)
        {
            _lastException = StackTraceUtility.ExtractStringFromException(e);
            _totalErrors++;
        }
    }

    /// <summary>
    ///     Retrieves translations for display strings used in the ProxySettingsWindow. This method typically initializes
    ///     or updates translated text elements like menu options and labels used within the window interface.
    /// </summary>
    protected virtual void GetTranslations()
    {
    }

    /// <inheritdoc />
    public override void PostOpen()
    {
        var modSettings = new List<FloatMenuOption>();

        foreach (var handle in LoadedModManager.ModHandles)
        {
            if (handle.SettingsCategory().NullOrEmpty()) continue;

            _hasSettings = true;
            modSettings.Add(new FloatMenuOption(handle.SettingsCategory(), () => DisplayMod(handle)));
        }

        _selectModText = VerseTranslations.SelectMod;

        if (!_hasSettings)
            _noSettingsFloatMenu = new FloatMenu(
                [new FloatMenuOption(VerseTranslations.NoConfigurableMods, action: null)]
            );

        _settingsFloatMenu = new FloatMenu(modSettings);
        GetTranslations();
    }

    /// <inheritdoc />
    public override void PreClose()
    {
        Find.WindowStack.TryRemove(typeof(Dialog_ModSettings));

        _mod.WriteSettings();
        base.PreClose();
    }

    private void DisplayMod(Verse.Mod handle)
    {
        if (handle == ModKit.Instance)
        {
            Find.WindowStack.Add(ModKit.Instance.SettingsWindow);
            Find.WindowStack.TryRemove(this, doCloseSound: false);

            return;
        }

        var window = new Dialog_ModSettings(handle);

        Find.WindowStack.TryRemove(this, doCloseSound: false);
        Find.WindowStack.Add(window);
    }

    /// <summary>
    ///     Opens the specified proxy settings window within the application. Ensures the window is not opened multiple
    ///     times and handles the state related to whether settings close was attempted.
    /// </summary>
    /// <param name="window">The instance of <see cref="ProxySettingsWindow" /> to be opened.</param>
    internal static void Open(ProxySettingsWindow window)
    {
        if (!window._settingsCloseAttempted)
        {
            Find.WindowStack.TryRemove(typeof(Dialog_ModSettings));
            window._settingsCloseAttempted = true;
        }

        if (!Find.WindowStack.IsOpen(window.GetType())) Find.WindowStack.Add(window);
    }
}