// MIT License
// 
// Copyright (c) 2024 sirrandoo
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

using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace ColonySync.Mod.Presentation;

/// <summary>A specialized worker for animating frames from a sprite sheet.</summary>
/// <param name="frames">The individual frames of the animation sprite sheet.</param>
/// <remarks>
///     This class doesn’t display (i.e., draw) the current frame of the animation to the screen; it's the view
///     layer's job to draw the current frame.
/// </remarks>
[PublicAPI]
public class SpriteSheetWorker(IReadOnlyList<Texture2D> frames)
{
    private int _index;
    private Timer? _timer;

    /// <summary>Gets the total number of frames in the sprite sheet.</summary>
    public int TotalFrames => frames.Count;

    /// <summary>The current frame of the sprite sheet being processed.</summary>
    public Texture CurrentFrame => frames[_index];

    /// <summary>Indicates whether the worker's internal timer is currently running or not.</summary>
    public bool Running { get; private set; }

    /// <summary>Starts the worker's internal animation timer.</summary>
    /// <param name="milliseconds">The number of milliseconds to wait between frame changes.</param>
    public void Start(int milliseconds)
    {
        ChangeTimer(milliseconds);
    }

    /// <summary>Stops the worker's internal <see cref="Timer" />.</summary>
    public void Stop()
    {
        ChangeTimer(Timeout.Infinite);
    }

    /// <summary>Sets the current frame being displayed.</summary>
    /// <param name="frame">The index of the current frame to display.</param>
    /// <remarks>The <paramref name="frame" /> parameter should be between 1 and <see cref="TotalFrames" /> .</remarks>
    public void SetFrame(int frame)
    {
        _index = (frame - 1) % frames.Count;

        ChangeTimer(Timeout.Infinite);
    }

    /// <summary>Moves to the next frame of the sprite sheet.</summary>
    public void NextFrame()
    {
        _index = (_index + 1) % frames.Count;
    }

    /// <summary>Moves to the last frame of the sprite sheet.</summary>
    public void ToLastFrame()
    {
        SetFrame(frames.Count);
    }

    /// <summary>Moves to the first frame of the sprite sheet.</summary>
    public void ToFirstFrame()
    {
        SetFrame(1);
    }

    /// <summary>Changes the interval of the worker's internal <see cref="Timer" />.</summary>
    /// <param name="period">The interval in milliseconds between the timer's tick events.</param>
    private void ChangeTimer(int period)
    {
        _timer ??= new Timer(
            w =>
            {
                if (w is SpriteSheetWorker worker) worker.NextFrame();
            },
            this,
            Timeout.Infinite,
            Timeout.Infinite
        );

        Running = period != Timeout.Infinite && _timer?.Change(dueTime: 0, period) == true;
    }

    /// <summary>Creates a new instance of a <see cref="SpriteSheetWorker" /> from a sprite sheet image.</summary>
    /// <param name="texture">The sprite sheet image to create an animation from.</param>
    /// <remarks>
    ///     This method extracts frames from the sprite sheet either left to right, or top to bottom, depending on the
    ///     orientation of the image. Extrapolation doesn’t support images that contain multiple rows if the image is
    ///     horizontally larger, or columns if the image is vertically larger. Sprite sheets must be one, continuous strip with
    ///     each frame immediately after the previous.
    /// </remarks>
    /// <returns>A new instance of <see cref="SpriteSheetWorker" /> initialized with the frames from the sprite sheet.</returns>
    public static SpriteSheetWorker CreateInstance(Texture2D texture)
    {
        var extrapolateFrames = ExtrapolateFrames(texture);

        return new SpriteSheetWorker(extrapolateFrames);
    }

    /// <summary>Creates a new instance of a <see cref="SpriteSheetWorker" /> with a collection of textures as frames.</summary>
    /// <param name="frames">The collection of textures used as animation frames.</param>
    /// <returns>A new instance of the <see cref="SpriteSheetWorker" /> class.</returns>
    public static SpriteSheetWorker CreateInstance(Texture2D[] frames)
    {
        return new SpriteSheetWorker(frames);
    }

    /// <summary>Creates a copy of the given texture.</summary>
    /// <param name="original">The original texture to copy from.</param>
    /// <returns>A new <see cref="Texture2D" /> object that is a copy of the original texture.</returns>
    private static Texture2D CopyImage(Texture original)
    {
        var tmpTexture = RenderTexture.GetTemporary(original.width, original.height);

        Graphics.Blit(original, tmpTexture);

        var previous = RenderTexture.active;
        RenderTexture.active = tmpTexture;

        var newTexture = new Texture2D(
            original.width, original.height, tmpTexture.graphicsFormat, TextureCreationFlags.None
        );

        newTexture.ReadPixels(new Rect(x: 0f, y: 0f, tmpTexture.width, tmpTexture.height), destX: 0, destY: 0);
        newTexture.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmpTexture);

        return newTexture;
    }

    /// <summary>Extrapolates individual frames from a single texture.</summary>
    /// <param name="texture">The sprite sheet texture to extrapolate frames from.</param>
    /// <returns>An array of individual frames extracted from the given sprite sheet texture.</returns>
    private static Texture2D[] ExtrapolateFrames(Texture2D texture)
    {
        var copy = CopyImage(texture);

        var imageSize = Mathf.Min(copy.width, copy.height);
        var isVertical = copy.height > copy.width;
        var totalFrames = Mathf.Max(copy.width, copy.height) / imageSize;
        var container = new Texture2D[totalFrames];

        for (var i = 0; i < totalFrames; i++)
        {
            var frame = new Texture2D(imageSize, imageSize, texture.format, mipChain: false, linear: true);
            var x = isVertical ? 0 : i * imageSize;
            var y = isVertical ? i * imageSize : 0;

            frame.SetPixels(copy.GetPixels(x, y, imageSize, imageSize));
            frame.Apply();

            container[i] = frame;
        }

        return container;
    }
}