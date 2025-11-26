/***************************************************************
 * File: GamePadInfo.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The GamePadInfo class represents and manages the state of a 
 *  connected gamepad device. It provides functionality for 
 *  detecting button presses, analog stick input, and handling 
 *  vibration feedback per player index.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CoreLibrary.Input;

/// <summary>
/// Represents and manages the state of a connected gamepad device.
/// Provides access to button states, thumbstick positions, triggers, 
/// and vibration control.
/// </summary>
public class GamePadInfo
{
    #region Fields

    private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the index of the player this gamepad is assigned to.
    /// </summary>
    public PlayerIndex PlayerIndex { get; }

    /// <summary>
    /// Gets the previous update cycle's gamepad state.
    /// </summary>
    public GamePadState PreviousState { get; private set; }

    /// <summary>
    /// Gets the current update cycle's gamepad state.
    /// </summary>
    public GamePadState CurrentState { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this gamepad is currently connected.
    /// </summary>
    public bool IsConnected => CurrentState.IsConnected;

    /// <summary>
    /// Gets the position of the left thumbstick.
    /// </summary>
    public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

    /// <summary>
    /// Gets the position of the right thumbstick.
    /// </summary>
    public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

    /// <summary>
    /// Gets the analog value of the left trigger.
    /// </summary>
    public float LeftTrigger => CurrentState.Triggers.Left;

    /// <summary>
    /// Gets the analog value of the right trigger.
    /// </summary>
    public float RightTrigger => CurrentState.Triggers.Right;

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="GamePadInfo"/> instance for the specified player index.
    /// </summary>
    /// <param name="playerIndex">The player index associated with this gamepad.</param>
    public GamePadInfo(PlayerIndex playerIndex)
    {
        PlayerIndex = playerIndex;
        PreviousState = new GamePadState();
        CurrentState = GamePad.GetState(playerIndex);
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Updates the gamepad state and handles vibration timing.
    /// </summary>
    /// <param name="gameTime">A snapshot of the current game time values.</param>
    public void Update(GameTime gameTime)
    {
        PreviousState = CurrentState;
        CurrentState = GamePad.GetState(PlayerIndex);

        if (_vibrationTimeRemaining > TimeSpan.Zero)
        {
            _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

            if (_vibrationTimeRemaining <= TimeSpan.Zero)
                StopVibration();
        }
    }

    /// <summary>
    /// Returns whether the specified gamepad button is currently pressed down.
    /// </summary>
    /// <param name="button">The gamepad button to check.</param>
    /// <returns><see langword="true"/> if the button is down; otherwise, <see langword="false"/>.</returns>
    public bool IsButtonDown(Buttons button) =>
        CurrentState.IsButtonDown(button);

    /// <summary>
    /// Returns whether the specified gamepad button is currently released.
    /// </summary>
    /// <param name="button">The gamepad button to check.</param>
    /// <returns><see langword="true"/> if the button is up; otherwise, <see langword="false"/>.</returns>
    public bool IsButtonUp(Buttons button) =>
        CurrentState.IsButtonUp(button);

    /// <summary>
    /// Determines whether the specified button was just pressed during this frame.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns><see langword="true"/> if the button was just pressed; otherwise, <see langword="false"/>.</returns>
    public bool WasButtonJustPressed(Buttons button) =>
        CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);

    /// <summary>
    /// Determines whether the specified button was just released during this frame.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns><see langword="true"/> if the button was just released; otherwise, <see langword="false"/>.</returns>
    public bool WasButtonJustReleased(Buttons button) =>
        CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);

    /// <summary>
    /// Sets the vibration strength and duration for the gamepad.
    /// </summary>
    /// <param name="strength">The vibration intensity (0.0f to 1.0f).</param>
    /// <param name="time">The duration of the vibration.</param>
    public void SetVibration(float strength, TimeSpan time)
    {
        _vibrationTimeRemaining = time;
        GamePad.SetVibration(PlayerIndex, strength, strength);
    }

    /// <summary>
    /// Stops all vibration for this gamepad.
    /// </summary>
    public void StopVibration()
    {
        GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
    }

    #endregion Public Methods
}
