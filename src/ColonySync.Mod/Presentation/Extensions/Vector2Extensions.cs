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

using JetBrains.Annotations;
using UnityEngine;

namespace ColonySync.Mod.Presentation.Extensions;

/// <summary>Provides extension methods for Unity's <see cref="Vector2" /> structure.</summary>
[PublicAPI]
public static class Vector2Extensions
{
    /// <summary>Sets the x component of the vector to the specified value.</summary>
    /// <param name="vector">The vector whose x component is to be set.</param>
    /// <param name="value">The new value for the x component.</param>
    /// <returns>The vector with the updated x component.</returns>
    public static Vector2 SetX(this Vector2 vector, float value)
    {
        vector.x = value;

        return vector;
    }

    /// <summary>Increments the X component of a Vector2 by a specified value.</summary>
    /// <param name="vector">The Vector2 instance whose X component is to be incremented.</param>
    /// <param name="value">The value by which to increment the X component.</param>
    /// <returns>A new Vector2 with the incremented X component.</returns>
    public static Vector2 IncrementX(this Vector2 vector, float value)
    {
        vector.x += value;

        return vector;
    }

    /// <summary>Sets the y component of the vector to the specified value.</summary>
    /// <param name="vector">The vector whose y component is to be set.</param>
    /// <param name="value">The value to set the y component to.</param>
    /// <returns>A new vector with the y component set to the specified value.</returns>
    public static Vector2 SetY(this Vector2 vector, float value)
    {
        vector.y = value;

        return vector;
    }

    /// <summary>Increments the y component of the vector by the specified value.</summary>
    /// <param name="vector">The vector whose y component will be incremented.</param>
    /// <param name="value">The value to increment the y component by.</param>
    /// <return>The vector with the incremented y component.</return>
    public static Vector2 IncrementY(this Vector2 vector, float value)
    {
        vector.y += value;

        return vector;
    }

    /// <summary>Resets the x and y components of the vector to zero.</summary>
    /// <param name="vector">The vector to be modified.</param>
    /// <returns>A new Vector2 instance with its x and y components set to zero.</returns>
    public static Vector2 AtZero(this Vector2 vector)
    {
        vector.x = 0;
        vector.y = 0;

        return vector;
    }
}