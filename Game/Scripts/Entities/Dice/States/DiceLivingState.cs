using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;

#nullable enable

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
    #endregion

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);

        Dice = Utils.GetValue(parameters, "player", Dice);

        if (Dice is null)
            throw new ArgumentNullException("Player is null in PlayerLivingState.");
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
        if (Dice is null || Dice.CurrentAnimation.CurrentCycle < 1) return;

        if (Dice.Hitbox.Velocity == Vector2.Zero)
        {
            if (Dice.DiceDirection == DiceDirections.UpLeft || Dice.DiceDirection == DiceDirections.DownRight)
            {
                // Dice is negative diagonal still.
                Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_negative_diagonal_idle_animation");
            }
            else if (Dice.DiceDirection == DiceDirections.UpRight || Dice.DiceDirection == DiceDirections.DownLeft)
            {
                // Dice is negative diagonal still.
                Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_positive_diagonal_idle_animation");
            }
            else
            {
                // Dice is still.
                Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_idle_animation");
            }
        } 
        else if (Dice.Hitbox.Velocity.X < 0 && Dice.Hitbox.Velocity.Y < 0)
        {
            // Dice is moving left up.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_negative_diagonal_animation");

            ReverseDiceAnimationIfNeeded(DiceDirections.UpLeft);
        }
        else if (Dice.Hitbox.Velocity.X > 0 && Dice.Hitbox.Velocity.Y > 0)
        {
            // Dice is moving right down.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_negative_diagonal_animation");

            ReverseDiceAnimationIfNeeded(DiceDirections.DownRight);
        }
        else if (Dice.Hitbox.Velocity.Y > 0 && Dice.Hitbox.Velocity.X < 0 )
        {
            // Dice is moving left down.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_positive_diagonal_animation");
            
            ReverseDiceAnimationIfNeeded(DiceDirections.DownLeft);
        }
        else if (Dice.Hitbox.Velocity.Y < 0 && Dice.Hitbox.Velocity.X > 0 )
        {
            // Dice is moving right up.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_positive_diagonal_animation");

            ReverseDiceAnimationIfNeeded(DiceDirections.UpRight);
        }
        else if (Dice.Hitbox.Velocity.X < 0)
        {
            // Dice is moving left.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_horizontal_animation");
            
            ReverseDiceAnimationIfNeeded(DiceDirections.Left);
        }
        else if (Dice.Hitbox.Velocity.X > 0)
        {
            // Dice is moving right.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_horizontal_animation");

            ReverseDiceAnimationIfNeeded(DiceDirections.Right);
        }
        else if (Dice.Hitbox.Velocity.Y < 0)
        {
            // Dice is moving up.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_vertical_animation");
            
            ReverseDiceAnimationIfNeeded(DiceDirections.Up);
        }
        else if (Dice.Hitbox.Velocity.Y > 0)
        {
            // Dice is moving down.
            Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}_vertical_animation");

            ReverseDiceAnimationIfNeeded(DiceDirections.Down);
        }
    }

    public void ReverseDiceAnimationIfNeeded(DiceDirections goingDirection)
    {
        if (Dice is null) return;

        if (goingDirection == Dice.DiceDirection) return;
        else if ((goingDirection is DiceDirections.Left or DiceDirections.Up or DiceDirections.UpLeft or DiceDirections.DownLeft) &&
                (Dice.DiceDirection is DiceDirections.Right or DiceDirections.Down or DiceDirections.DownRight or DiceDirections.UpRight)) Dice.CurrentAnimation.ReverseAnimation();
        else if ((Dice.DiceDirection is DiceDirections.Left or DiceDirections.Up or DiceDirections.UpLeft or DiceDirections.DownLeft) &&
                (goingDirection is DiceDirections.Right or DiceDirections.Down or DiceDirections.DownRight or DiceDirections.UpRight)) Dice.CurrentAnimation.ReverseAnimation();

        Dice.DiceDirection = goingDirection;
    }
    #endregion Methods
}