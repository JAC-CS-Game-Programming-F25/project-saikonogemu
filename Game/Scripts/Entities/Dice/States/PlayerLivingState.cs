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
/// Represents the state of the player in living form.
/// </summary>
public class PlayerLivingState : DiceLivingState
{
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
        if (Dice!.IsFrozen)
            return;

        base.Update(gameTime);

        CheckKeyboardInput();
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
    /// Checks for movement keys and modifies velocity.
    /// </summary>
    private void CheckKeyboardInput()
    {
        Vector2 selectDelta = Vector2.Zero;

        // If the Up keys are down, move the player up on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.Up) || Core.Input.Keyboard.IsKeyDown(Keys.W)) selectDelta.Y -= 1;

        // if the Down keys are down, move the player down on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.Down) || Core.Input.Keyboard.IsKeyDown(Keys.S)) selectDelta.Y += 1;

        // If the Left keys are down, move the player left on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.Left) || Core.Input.Keyboard.IsKeyDown(Keys.A)) selectDelta.X -= 1;

        // If the Right keys are down, move the player right on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.Right) || Core.Input.Keyboard.IsKeyDown(Keys.D)) selectDelta.X += 1;

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