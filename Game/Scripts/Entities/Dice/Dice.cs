/***************************************************************
 * File: Dice.cs
 * Author: FarLostBrand
 * Date: December 4, 2025
 * 
 * Summary:
 *  The Dice class represents the main type of entity in the
 *  game. It extends the base GameEntity class and inherits all 
 *  functionality related to animation, physics, and state 
 *  management. Dice specific configuration values may be supplied
 *  through the diceDefinition dictionary during construction.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pleasing;

#nullable enable

namespace Game.Scripts.Entities.Dice;

/// <summary>
/// Creates a new dice entity instance.
/// This serves as a generic for the dice to come.
/// </summary>
/// <param name="content">The content manager used by the scene to load in content.</param>
/// <param name="diceDefinition">All the dice specific parameters.</param>
public class Dice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : GameEntity(content, diceDefinition)
{
    #region Properties
    // This is calculated by doing pixels/scale.
    public const float NORMAL_OFFSET = 1.67f;
    public const float DIAGONAL_OFFSET = 3.33f;
    public DiceDirections DiceDirection {get; set;} = DiceDirections.Idle;
    public float DiceOpacity {get; set; } = 1.0f;

    #endregion Properties

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        CurrentAnimation.Scale = Scale;
    }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        CurrentAnimation.Color = Color.White * DiceOpacity;
    }

    #endregion Update and Draw

    #region Methods
    /// <summary>
    /// Called when the dice loses a life.
    /// </summary>
    public void LoseLife()
    {
        Health--;

        if (Health <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// Called when the dice hits 0 hp.
    /// </summary>
    public void Death()
    {
        // FIXME: Move to DiceDyingState.
        IsDead = true;

        // We want it to run once.
        UpdateAnimation(GetDiceTypeTexture() + $"_death_animation", 1);
    }

    /// <summary>
    /// Gets the name preset of file paths for that dice.
    /// </summary>
    /// <returns>Returns the name preset.</returns>
    public string GetDiceTypeTexture()
    {
        // Get the end path of the file.
        // e.g. player_dice_atlas.xml
        Match? match = Regex.Match(EntityTexture.FileName, @"([^/]+)_atlas\.xml$");

        // If there was a match.
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return "FailedFileName";
    }

    /// <summary>
    /// Gets the name preset of animation name for that dice without the dice type.
    /// </summary>
    /// <returns>Returns the animation name for that dice without the dice type.</returns>
    public string GetAnimationTypeWithoutDice()
    {
        // Get the end of the animation name.
        // e.g. player_dice_atlas.xml
        Match match = Regex.Match(CurrentAnimationName, @"(?<=dice).*$");

        // If there was a match.
        if (match.Success)
        {
            return match.Groups[0].Value;
        }

        return "FailedFileName";
    }

    /// <summary>
    /// Tweens the opacity of the dice.
    /// </summary>
    /// <param name="duration">How long the tweening should take.</param>
    /// <param name="targetOpacity">The target finishing opacity.</param>
    /// <param name="easing">The easing we want to use, Linear is the default.</param>
    /// <returns>The tweening timeline so we can stop/repeat the tween.</returns>
    public TweenTimeline TweenOpacity(float duration, float targetOpacity, EasingFunction? easing = null)
    {
        // This will make sure the opacity is a valid float.
        targetOpacity = MathHelper.Clamp(targetOpacity, 0f, 1f);

        // Creates a tweening timeline (makes it so we can repeat/stop it).
        TweenTimeline timeline = Tweening.NewTimeline();
        timeline.AdaptiveDuration = true;

        TweenableProperty<float> opacityProp = timeline.AddFloat(this, nameof(DiceOpacity));

        // We'll use Linear easing by default.
        EasingFunction ease = easing ?? Easing.Linear;

        // Add the start and end frames (like checkpoints, super helpful for linked tweens).
        opacityProp.AddFrame(0f, DiceOpacity, ease);
        opacityProp.AddFrame(duration, targetOpacity, ease);

        return timeline;
    }
    #endregion Methods
}