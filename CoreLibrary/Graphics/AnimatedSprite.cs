/***************************************************************
 * File: AnimatedSprite.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The AnimatedSprite class extends the base Sprite functionality
 *  to support frame-based animations. It cycles through frames in
 *  a provided Animation instance based on elapsed time, updating
 *  the displayed region automatically.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using Microsoft.Xna.Framework;

namespace CoreLibrary.Graphics;

/// <summary>
/// Represents a sprite that supports frame-based animation playback.
/// </summary>
public class AnimatedSprite : Sprite
{
    #region Fields
    private readonly int _totalCycles;
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;

    #endregion Fields

    #region Properties

    public int CurrentCycle {get; private set;} = 0;

    /// <summary>
    /// Gets or sets the animation assigned to this animated sprite.
    /// Setting a new animation resets the displayed frame to the first one.
    /// </summary>
    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }
    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedSprite"/> class.
    /// </summary>
    public AnimatedSprite() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimatedSprite"/> class
    /// using the specified animation.
    /// </summary>
    /// <param name="animation">The animation to assign to this sprite.</param>
    /// <param name="totalCycles">The amount of times you want the animation to run. Default is the max possible.</param>
    public AnimatedSprite(Animation animation, int totalCycles = int.MaxValue)
    {
        Animation = animation;
        _totalCycles = totalCycles;
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Updates the animated sprite by progressing through its animation frames
    /// based on elapsed game time.
    /// </summary>
    /// <param name="gameTime">A snapshot of the game's timing values.</param>
    public void Update(GameTime gameTime)
    {
        // Exits if we hit the cycle limit.
        if (CurrentCycle >= _totalCycles)
            return;

        // Accumulate elapsed time.
        _elapsed += gameTime.ElapsedGameTime;

        // Check if enough time has passed to move to the next frame.
        if (_elapsed >= _animation.Delay)
        {
            _elapsed -= _animation.Delay;
            _currentFrame++;

            // Loop back to the start of the animation if we've reached the end.
            if (_currentFrame >= _animation.Frames.Count)
            {
                CurrentCycle++;

                // Exits if we hit the cycle limit.
                if (CurrentCycle >= _totalCycles)
                    return;

                _currentFrame = 0;
            }

            // Update the sprite’s region to reflect the current frame.
            Region = _animation.Frames[_currentFrame];
        }
    }

    /// <summary>
    /// Reverse the animation by swapping the order of the frames.
    /// </summary>
    public void ReverseAnimation()
    {
        Animation.ReverseAnimation();
    }

    /// <summary>
    /// Clones the AnimatedSprite instance (deep clone).
    /// </summary>
    /// <returns>Returns the cloned item.</returns>
    public virtual AnimatedSprite Clone()
    {
        AnimatedSprite clone = (AnimatedSprite)MemberwiseClone();
        clone.Animation = Animation.Clone();
        return clone;
    }

    /// <summary>
    /// Checks whether the animation is done.
    /// </summary>
    /// <returns>Returns whether the animation is done.</returns>
    public bool IsDone()
    {
        return CurrentCycle >= _totalCycles;
    }
    #endregion Public Methods
}
