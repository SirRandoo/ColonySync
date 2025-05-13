// MIT License
//
// Copyright (c) 2024 SirRandoo
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

using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Bootloader;

/// <summary>Represents the bootstrap module for ColonySync.</summary>
/// <remarks>
///     This class is responsible for initializing ColonySync and handling its settings
///     window.
/// </remarks>
[StaticConstructorOnStartup]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
internal class BootstrapMod : Verse.Mod
{
    private static readonly Color DescriptionTextColor = new(r: 0.72f, g: 0.72f, b: 0.72f);

    public BootstrapMod(ModContentPack content) : base(content)
    {
        Instance = this;
    }

    /// <summary>Gets the singleton instance of the <see cref="BootstrapMod" /> class.</summary>
    /// <value>The static instance of <see cref="BootstrapMod" />.</value>
    public static BootstrapMod Instance { get; private set; } = null!;

    /// <inheritdoc />
    public override string? SettingsCategory()
    {
        return ModLister.GetActiveModWithIdentifier("com.sirrandoo.colonysync") != null
            ? "ColonySync - Bootstrapper"
            : null;
    }

    /// <inheritdoc />
    public override void DoSettingsWindowContents(Rect inRect)
    {
        var oldColor = GUI.color;
        var oldFont = Text.Font;
        var oldAnchor = Text.Anchor;

        Text.Font = GameFont.Medium;
        GUI.color = DescriptionTextColor;
        Text.Anchor = TextAnchor.MiddleCenter;

        Widgets.Label(inRect, "ColonySync.Bootstrap.Info".TranslateSimple());

        Text.Font = oldFont;
        GUI.color = oldColor;
        Text.Anchor = oldAnchor;
    }
}