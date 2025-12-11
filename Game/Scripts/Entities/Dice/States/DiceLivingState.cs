using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;

#nullable enable

namespace Game.Scripts.Entities.Dice.States;

/// <summary>
/// Represents the state of the dice in living form.
/// </summary>
public class DiceLivingState : State
{
    #region Properties
    /// <summary>
    /// The player instance used in the states.
    /// </summary>
    protected Dice? Dice { get; private set; }
    #endregion Properties

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);

        Dice = Utils.GetValue(parameters, "dice", Dice);

        if (Dice is null)
            throw new ArgumentNullException("dice is null in PlayerLivingState.");
    }

    /// <summary>
    /// Called when exiting this State.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime">The GameTime of the game.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Updates the animation sprite based on the direction the dice is moving.
        UpdateAnimationRelativeToMovement();
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime">The GameTime of the game.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
    #endregion Lifecycle Methods

    #region Methods
    /// <summary>
    /// Updates the animation sprite based on the direction the dice is moving.
    /// </summary>
    public void UpdateAnimationRelativeToMovement()
    {
        if (Dice is null) return;

        if (Dice.Hitbox.Velocity == Vector2.Zero)
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.Idle) return;

            if (Dice.DiceDirection == DiceDirections.UpLeft || Dice.DiceDirection == DiceDirections.DownRight)
            {
                // Dice is negative diagonal still.
                Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_negative_diagonal_idle_animation");
                Dice.CurrentAnimation.Origin = new Vector2(Dice.DIAGONAL_OFFSET, Dice.DIAGONAL_OFFSET) * Dice.Scale;
                Dice.DiceDirection = DiceDirections.Idle;
            }
            else if (Dice.DiceDirection == DiceDirections.UpRight || Dice.DiceDirection == DiceDirections.DownLeft)
            {
                // Dice is negative diagonal still.
                Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_positive_diagonal_idle_animation");
                Dice.CurrentAnimation.Origin = new Vector2(Dice.DIAGONAL_OFFSET, Dice.DIAGONAL_OFFSET) * Dice.Scale;
                Dice.DiceDirection = DiceDirections.Idle;
            }
            else
            {
                // Dice is still.
                Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_idle_animation");
                Dice.CurrentAnimation.Origin = new Vector2(Dice.NORMAL_OFFSET, Dice.NORMAL_OFFSET) * Dice.Scale;
                Dice.DiceDirection = DiceDirections.Idle;
            }
        } 
        else if (Dice.Hitbox.Velocity.X < 0 && Dice.Hitbox.Velocity.Y < 0)
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.UpLeft) return;

            // Dice is moving left up.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_up_left_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.DIAGONAL_OFFSET, Dice.DIAGONAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.UpLeft;
        }
        else if (Dice.Hitbox.Velocity.X > 0 && Dice.Hitbox.Velocity.Y > 0)
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.DownRight) return;

            // Dice is moving right down.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_down_right_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.DIAGONAL_OFFSET, Dice.DIAGONAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.DownRight;
        }
        else if (Dice.Hitbox.Velocity.Y > 0 && Dice.Hitbox.Velocity.X < 0 )
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.DownLeft) return;
            
            // Dice is moving left down.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_down_left_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.DIAGONAL_OFFSET, Dice.DIAGONAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.DownLeft;
        }
        else if (Dice.Hitbox.Velocity.Y < 0 && Dice.Hitbox.Velocity.X > 0 )
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.UpRight) return;

            // Dice is moving right up.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_up_right_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.DIAGONAL_OFFSET, Dice.DIAGONAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.UpRight;
        }
        else if (Dice.Hitbox.Velocity.X < 0)
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.Left) return;

            // Dice is moving left.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_left_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.NORMAL_OFFSET, Dice.NORMAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.Left;
        }
        else if (Dice.Hitbox.Velocity.X > 0)
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.Right) return;

            // Dice is moving right.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_right_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.NORMAL_OFFSET, Dice.NORMAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.Right;
        }
        else if (Dice.Hitbox.Velocity.Y < 0)
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.Up) return;

            // Dice is moving up.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_up_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.NORMAL_OFFSET, Dice.NORMAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.Up;
        }
        else if (Dice.Hitbox.Velocity.Y > 0)
        {
            // Returns if we are already animating the correct way.
            if (Dice.DiceDirection == DiceDirections.Down) return;

            // Dice is moving down.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_down_animation");
            Dice.CurrentAnimation.Origin = new Vector2(Dice.NORMAL_OFFSET, Dice.NORMAL_OFFSET) * Dice.Scale;
            Dice.DiceDirection = DiceDirections.Down;
        }
    }
    #endregion Methods
}