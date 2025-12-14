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
using CoreLibrary;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    #region Backing Fields
    private float _iFramesTimer = 0f;
    #endregion Backing Fields

    #region Properties
    // This is calculated by doing pixels/scale.
    public const float COLLISION_POWER = 2.0f;
    public const float NORMAL_OFFSET = 1.67f;
    public const float DIAGONAL_OFFSET = 3.33f;
    public const float DECELERATION = 20f; 
    private const float IFRAMES_DURATION = 1.0f;
    public DiceDirections DiceDirection {get; set;} = DiceDirections.Idle;
    public float DiceOpacity {get; set;} = 1.0f;
    public bool IsHavingKnockback {get; set;}
    public bool IsFrozen {get; set;}
    public bool IsLosingLife {get; set;}

    #endregion Properties

    #region Update and DrawLoseLife

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
        if (IsHavingKnockback)
        {
            HandleCollisionDeceleration();
            return;
        }

        if (IsLosingLife)
            HandleIFrames(gameTime);

        base.Update(gameTime);
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
        if (IsLosingLife) return;

        Health -= 1;

        if (Health <= 0)
        {
            // Prevents over shooting.
            Health = 0;
            Death();
        }
        else
        {
            UpdateAnimation(GetDiceTypeTexture() + $"_dot{Health}" + GetAnimationTypeWithoutDice());
            IsLosingLife = true;
        }
        // Audio.
        int sfxNumber = new Random().Next(0, 3);

        switch (sfxNumber)
        {
            case 0:
                Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/hit1"));
            break;

            case 1:
                Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/hit2"));
            break;

            case 2:
                Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/hit3"));
            break;
        }
    }

    /// <summary>
    /// Performs knockback on dice.
    /// </summary>
    public void Knockback()
    {
        IsHavingKnockback = true;
        FlattenSpeed();
        Hitbox.Velocity *= -COLLISION_POWER;
    }

    /// <summary>
    /// Called when the dice hits 0 hp.
    /// </summary>
    public void Death()
    {
        ChangeState("DiceDyingState", new Dictionary<string, object> { ["dice"] = this });
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
        Match match = Regex.Match(CurrentAnimationName, @"(?<=dice_dot\d).*$");

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

    /// <summary>
    /// Check to see if collision with other dice is true.
    /// </summary>
    /// <param name="otherDice">The other dice to check collision with.</param>
    /// <returns> Whether the dice collided.</returns>
    public bool DidCollideWithOtherDice(Dice otherDice)
    {
        return Hitbox.Collider.Intersects(otherDice.Hitbox.Collider);
    }

    /// <summary>
    /// Makes the knockback slowly end by adjusting the dice's velocity.
    /// </summary>
    private void HandleCollisionDeceleration()
    {
        // Progressively get to dice's speed.
        ClampAxisTowardsSpeed(ref Hitbox.Velocity.X, DECELERATION);
        ClampAxisTowardsSpeed(ref Hitbox.Velocity.Y, DECELERATION);

        // Check to see if the knockback is over.
        if (Math.Abs(Hitbox.Velocity.X) <= Speed && Math.Abs(Hitbox.Velocity.Y) <= Speed)
            IsHavingKnockback = false;
    }

    /// <summary>
    /// "Clamps" the velocity of a specific axis towards the dice's speed.
    /// </summary>
    /// <param name="axisVelocity">The velocity of the axis to clamp.</param>
    public void ClampAxisTowardsSpeed(ref float axisVelocity, float deceleration)
    {
        // If the axis' velocity is greater than the dice's speed.
        if (Math.Abs(axisVelocity) > Speed)
        {
            // Move towards the dice speed.
            axisVelocity -= Math.Sign(axisVelocity) * deceleration;

            // If we overshoot we adjust the velocity back to the dice's speed.
            if (Math.Abs(axisVelocity) < Speed)
                axisVelocity = Math.Sign(axisVelocity) * Speed;
        }
    }

    /// <summary>
    /// Resets the speed to it's directional equivalent.
    /// </summary>
    public void FlattenSpeed()
    {
        Vector2 selectDelta = Vector2.Zero;

        if (DiceDirection is DiceDirections.Up or DiceDirections.UpLeft or DiceDirections.UpRight) selectDelta.Y -= 1;

        if (DiceDirection is DiceDirections.Down or DiceDirections.DownLeft or DiceDirections.DownRight) selectDelta.Y += 1;

        if (DiceDirection is DiceDirections.Left or DiceDirections.UpLeft or DiceDirections.DownLeft) selectDelta.X -= 1;

        if (DiceDirection is DiceDirections.Right or DiceDirections.UpRight or DiceDirections.DownRight) selectDelta.X += 1;

        // We normalize if it's possible (this makes diagonals the same speed as horizontals/verticals).
        if (selectDelta != Vector2.Zero)
        {
            selectDelta = Vector2.Normalize(selectDelta) * Speed;
        }

        // Modifies the die's velocity.
        Hitbox.Velocity = selectDelta;
    }

    /// <summary>
    /// Formally Deletes the Dice.
    /// </summary>
    public void Delete()
    {
        // Removes hanging thread.
        if (Hitbox != null)
        {
            PhysicsManager.Instance.RemoveRigidbody(Hitbox);
        }
    }

    /// <summary>
    /// Handles the iframes when hit.
    /// </summary>
    /// <param name="gameTime">The gameTime of the Game.</param>
    private void HandleIFrames(GameTime gameTime)
    {
        _iFramesTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        DiceOpacity = DiceOpacity == 1.0f ? 0.5f : 1.0f;

        if (_iFramesTimer >= IFRAMES_DURATION)
        {
            _iFramesTimer = 0;
            IsLosingLife = false;
            DiceOpacity = 1.0f;
        }
    }

    /// <summary>
    /// Get the opposite direction side based on the passed collision side.
    /// Uses a fancy switch state function suggested by vs code.
    /// </summary>
    /// <param name="side">The side to get the opposite of.</param>
    /// <returns>The opposite side.</returns>
    public static DiceDirections OppositeDiceDirection(DiceDirections side) => side switch
    {
        DiceDirections.Right => DiceDirections.Left,
        DiceDirections.Left => DiceDirections.Right,
        DiceDirections.Up => DiceDirections.Down,
        DiceDirections.Down => DiceDirections.Up,
        DiceDirections.UpRight => DiceDirections.DownLeft,
        DiceDirections.UpLeft => DiceDirections.DownRight,
        DiceDirections.DownRight => DiceDirections.UpLeft,
        DiceDirections.DownLeft => DiceDirections.UpRight,
        _ => DiceDirections.Idle
    };
    #endregion Methods
}