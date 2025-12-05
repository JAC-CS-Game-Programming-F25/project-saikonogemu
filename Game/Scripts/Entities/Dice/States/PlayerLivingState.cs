using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreLibrary;
using CoreLibrary.Input;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#nullable enable

/// <summary>
/// Represents the state of the player in living form.
/// </summary>
public class PlayerLivingState : DiceLivingState
{
    #region Fields
    private const float MOVEMENT_SPEED = 200.0f;
    #endregion Fields

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

        // If the W or Up keys are down, move the slime up on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.W)) selectDelta.Y -= MOVEMENT_SPEED;

        // if the S or Down keys are down, move the slime down on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.S)) selectDelta.Y += MOVEMENT_SPEED;

        // If the A or Left keys are down, move the slime left on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.A)) selectDelta.X -= MOVEMENT_SPEED;

        // If the D or Right keys are down, move the slime right on the screen.
        if (Core.Input.Keyboard.IsKeyDown(Keys.D)) selectDelta.X += MOVEMENT_SPEED;

        // Modifies the player's velocity.
        Dice!.Hitbox.Velocity = selectDelta;
    }

    private void CheckGamePadInput()
    {
        GamePadInfo gamePadOne = Core.Input.GamePads[(int)PlayerIndex.One];

        // TODO: implement gamepad controls
    }

    #endregion Input Handling
}