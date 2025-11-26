/***************************************************************
 * File: InputManager.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The InputManager class serves as a centralized input handler 
 *  for the game, managing the state of keyboard, mouse, and 
 *  multiple gamepads. It provides unified access to input data 
 *  and ensures all devices are updated each frame.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using Microsoft.Xna.Framework;

namespace CoreLibrary.Input;

/// <summary>
/// Manages input devices including keyboard, mouse, and gamepads.
/// Provides a unified interface to update and access their states.
/// </summary>
public class InputManager
{
    #region Properties

    /// <summary>
    /// Gets the state information of keyboard input.
    /// </summary>
    public KeyboardInfo Keyboard { get; private set; }

    /// <summary>
    /// Gets the state information of mouse input.
    /// </summary>
    public MouseInfo Mouse { get; private set; }

    /// <summary>
    /// Gets the state information of connected gamepads.
    /// </summary>
    public GamePadInfo[] GamePads { get; private set; }

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InputManager"/> class.
    /// Sets up keyboard, mouse, and gamepad input tracking.
    /// </summary>
    public InputManager()
    {
        Keyboard = new KeyboardInfo();
        Mouse = new MouseInfo();

        GamePads = new GamePadInfo[4];
        for (int i = 0; i < 4; i++)
            GamePads[i] = new GamePadInfo((PlayerIndex)i);
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Updates the state of all input devices including keyboard, mouse, and gamepads.
    /// Should be called once per frame to ensure input data is current.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of the current game timing values.</param>
    public void Update(GameTime gameTime)
    {
        Keyboard.Update();
        Mouse.Update();

        for (int i = 0; i < GamePads.Length; i++)
            GamePads[i].Update(gameTime);
    }

    #endregion Public Methods
}
