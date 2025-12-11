/***************************************************************
 * File: KeyboardInfo.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The KeyboardInfo class represents and manages the state of 
 *  keyboard input. It provides functionality for detecting 
 *  key presses, releases, and transitions between frames.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using Microsoft.Xna.Framework.Input;

namespace CoreLibrary.Input;

/// <summary>
/// Represents and manages the current and previous states of keyboard input.
/// Provides methods to check key status and detect key press transitions.
/// </summary>
public class KeyboardInfo
{
    #region Properties

    /// <summary>
    /// Gets the state of keyboard input during the previous update cycle.
    /// </summary>
    public KeyboardState PreviousState { get; private set; }

    /// <summary>
    /// Gets the state of keyboard input during the current input cycle.
    /// </summary>
    public KeyboardState CurrentState { get; private set; }

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardInfo"/> class.
    /// </summary>
    public KeyboardInfo()
    {
        PreviousState = new KeyboardState();
        CurrentState = Keyboard.GetState();
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Updates the current and previous keyboard states.
    /// Should be called once per frame to track input changes.
    /// </summary>
    public void Update()
    {
        PreviousState = CurrentState;
        CurrentState = Keyboard.GetState();
    }

    /// <summary>
    /// Determines whether the specified key is currently pressed down.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if the key is down; otherwise, <see langword="false"/>.</returns>
    public bool IsKeyDown(Keys key) =>
        CurrentState.IsKeyDown(key);

    /// <summary>
    /// Determines whether the specified key is currently released.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if the key is up; otherwise, <see langword="false"/>.</returns>
    public bool IsKeyUp(Keys key) =>
        CurrentState.IsKeyUp(key);

    /// <summary>
    /// Determines whether the specified key was just pressed during this frame.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if the key was just pressed; otherwise, <see langword="false"/>.</returns>
    public bool WasKeyJustPressed(Keys key) =>
        CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);

    /// <summary>
    /// Determines whether the specified key was just released during this frame.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><see langword="true"/> if the key was just released; otherwise, <see langword="false"/>.</returns>
    public bool WasKeyJustReleased(Keys key) =>
        CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);

    /// <summary>
    /// True if any key is currently held down.
    /// </summary>
    public bool AnyKeyDown =>
        CurrentState.GetPressedKeys().Length > 0;

    /// <summary>
    /// True if any key was pressed this frame (transition from Up → Down).
    /// </summary>
    public bool AnyKeyJustPressed
    {
        get
        {
            var current = CurrentState.GetPressedKeys();
            foreach (var key in current)
                if (PreviousState.IsKeyUp(key))
                    return true;

            return false;
        }
    }

    /// <summary>
    /// True if any key was released this frame (transition from Down → Up).
    /// </summary>
    public bool AnyKeyJustReleased
    {
        get
        {
            var previous = PreviousState.GetPressedKeys();
            foreach (var key in previous)
                if (CurrentState.IsKeyUp(key))
                    return true;

            return false;
        }
    }
    #endregion Public Methods
}
