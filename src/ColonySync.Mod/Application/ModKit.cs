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

using ColonySync.Mod.Presentation.Windows;
using JetBrains.Annotations;
using UnityEngine;
using Verse;

namespace ColonySync.Mod.Application;

/// <summary>Represents the core mod class for interacting with the mod content in the framework.</summary>
/// <remarks>
///     The <see cref="ModKit" /> class extends <see cref="Verse.Mod" /> and provides functionality to manage
///     mod-specific behaviors, including a settings window and general configuration for the associated mod.
/// </remarks>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
public sealed class ModKit : Verse.Mod
{
    public ModKit(ModContentPack content) : base(content)
    {
        Instance = this;
    }

    /// <summary>Represents the singleton instance of the <see cref="ModKit" /> class.</summary>
    /// <remarks>
    ///     This property provides global access to the only instance of the <see cref="ModKit" /> class, ensuring that
    ///     only one instance is active at any given time. The instance is initialized during the construction of the
    ///     <see cref="ModKit" /> object.
    /// </remarks>
    [PublicAPI]
    public static ModKit Instance { get; private set; } = null!;

    internal SettingsWindow SettingsWindow => new(this);

    /// <inheritdoc />
    public override string SettingsCategory()
    {
        return Content.Name;
    }

    /// <inheritdoc />
    public override void DoSettingsWindowContents(Rect inRect)
    {
        ProxySettingsWindow.Open(SettingsWindow);
    }
}
