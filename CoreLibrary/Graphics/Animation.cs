/***************************************************************
 * File: Animation.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Animation class represents a frame-based animation sequence
 *  composed of multiple texture regions. Each frame is displayed for
 *  a set duration before progressing to the next, allowing smooth
 *  sprite animations when used with AnimatedSprite.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;

namespace CoreLibrary.Graphics;

/// <summary>
/// Represents a frame-based animation made up of a sequence of texture regions
/// displayed in order with a configurable delay between frames.
/// </summary>
public class Animation
{
    #region Properties

    /// <summary>
    /// Gets or sets the texture regions that make up the frames of this animation.
    /// The order of the regions within the collection determines the playback order.
    /// </summary>
    public List<TextureRegion> Frames { get; set; }

    /// <summary>
    /// Gets or sets the amount of time to delay between each frame before moving
    /// to the next one in the sequence.
    /// </summary>
    public TimeSpan Delay { get; set; }

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Animation"/> class
    /// with default values (100ms delay and an empty frame list).
    /// </summary>
    public Animation()
    {
        Frames = new List<TextureRegion>();
        Delay = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Animation"/> class
    /// using the specified frames and delay.
    /// </summary>
    /// <param name="frames">An ordered collection of texture regions representing the animation frames.</param>
    /// <param name="delay">The delay duration between frame transitions.</param>
    public Animation(List<TextureRegion> frames, TimeSpan delay)
    {
        Frames = frames;
        Delay = delay;
    }

    #endregion Constructors

    #region Methods
    /// <summary>
    /// Reverse the animation by swapping the order of the frames.
    /// </summary>
    public void ReverseAnimation()
    {
        Frames.Reverse();
    }

    /// <summary>
    /// Clones the Animation instance (deep clone).
    /// </summary>
    /// <returns>Returns the cloned item.</returns>
    public Animation Clone()
    {
        return (Animation)MemberwiseClone();
    }
    #endregion Methods
}
