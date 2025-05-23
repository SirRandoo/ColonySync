﻿// MIT License
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

namespace ColonySync.Mod.Presentation;

/// <summary>
///     Provides a collection of predefined colors for use in the library.
/// </summary>
[PublicAPI]
public static class UxColors
{
    // ReSharper disable once IdentifierTypo
    public const string RedishPinkHex = "#FF87C1";

    public static readonly Color Transparent = new(r: 1f, g: 1f, b: 1f, a: 0f);
    public static readonly Color MostlyTransparent = new(r: 1f, g: 1f, b: 1f, a: 0.25f);
    public static readonly Color HalfTransparent = new(r: 1f, g: 1f, b: 1f, a: 0.5f);
    public static readonly Color SomewhatTransparent = new(r: 1f, g: 1f, b: 1f, a: 0.75f);

    // This will never be white.
    public static readonly Color RedishPink =
        ColorUtility.TryParseHtmlString(RedishPinkHex, out var color) ? color : Color.white;
}
