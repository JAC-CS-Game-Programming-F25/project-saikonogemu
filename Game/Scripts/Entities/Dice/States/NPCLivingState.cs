using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreLibrary;
using CoreLibrary.Input;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#nullable enable

namespace Game.Scripts.Entities.Dice.States;

/// <summary>
/// Represents the state of the npc dice in living form.
/// </summary>
public class NPCLivingState : DiceLivingState
{
    #region Constants
    const double DIRECTION_CHANGE_CHANCE = 0.01;
    #endregion Constants

    #region Backing Fields
    private DiceDirections _newDirection;
    #endregion Backing Fields

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);
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

        HandleMovement();
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

    #region Input Handling

    /// <summary>
    /// Handles NPC movement.
    /// </summary>
    private void HandleMovement()
    {
        UpdateDirection();
        UpdateVelocity();
    }

    private void UpdateDirection()
    {
        Random random = new Random();
        
        // Chance of not changing directions.
        if (random.NextDouble() > DIRECTION_CHANGE_CHANCE)
            return;

        _newDirection = (DiceDirections)random.Next((int)DiceDirections.Left, (int)DiceDirections.Idle);
    }

    /// <summary>
    /// Updates velocity accordingly to dice direction.
    /// </summary>
    private void UpdateVelocity()
    {
        Vector2 selectDelta = Vector2.Zero;

        if (_newDirection is DiceDirections.Up or DiceDirections.UpLeft or DiceDirections.UpRight) selectDelta.Y -= 1;

        if (_newDirection is DiceDirections.Down or DiceDirections.DownLeft or DiceDirections.DownRight) selectDelta.Y += 1;

        if (_newDirection is DiceDirections.Left or DiceDirections.UpLeft or DiceDirections.DownLeft) selectDelta.X -= 1;

        if (_newDirection is DiceDirections.Right or DiceDirections.UpRight or DiceDirections.DownRight) selectDelta.X += 1;

        // We normalize if it's possible (this makes diagonals the same speed as horizontals/verticals).
        if (selectDelta != Vector2.Zero)
        {
            selectDelta = Vector2.Normalize(selectDelta) * Dice!.Speed;
        }

        // Modifies the player's velocity.
        Dice!.Hitbox.Velocity = selectDelta;
    }
    #endregion Input Handling
}