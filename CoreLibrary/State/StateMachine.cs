/***************************************************************
 * File: StateMachine.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The StateMachine class manages a collection of game states.
 *  It maintains the active state, supports adding and switching
 *  between states, and routes update and draw calls to the
 *  currently active state. Each State encapsulates its own
 *  behavior, and the StateMachine ensures proper Enter/Exit
 *  transitions.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CoreLibrary;

#nullable enable

/// <summary>
/// Stores the different states and helps manage it.
/// </summary>
public class StateMachine
{
    #region Properties
    /// <summary>
    /// The stores states.
    /// </summary>
    private readonly Dictionary<string, State> _states = new();

    /// <summary>
    /// The current state (value).
    /// </summary>
    private State? _currentState;

    /// <summary>
    /// The current state name (key).
    /// </summary>
    public string? CurrentStateName { get; private set; }
    #endregion Properties

    #region Methods
    /// <summary>
    /// Adds provided state to the state machine.
    /// </summary>
    /// <param name="name">The name of the state you would like to add to the state machine (key).</param>
    /// <param name="state">The state you would like to add to the state machine (value).</param>
    /// <param name="enterParameters">The parameters passed to the state when it's enter is called.</param>
    /// <exception cref="ArgumentNullException">Thrown when invalid State is provided.</exception>
    public void Add(string name, State state, Dictionary<string, object>? enterParameters = null)
    {
        // Throws if the given state is an invalid state.
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        // Adds the state to the state machine.
        _states[name] = state;

        // If there is no current state, sets it as the current state.
        if (_currentState == null)
        {
            _currentState = state;
            CurrentStateName = name;

            // Starts the current state.
            state.Enter(enterParameters);
        }
    }

    /// <summary>
    /// Changes current state to provided state.
    /// </summary>
    /// <param name="name">The name of the state you would like to change to.</param>
    /// <param name="enterParameters">The parameters passed to the state when it's enter is called.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Change(string name, Dictionary<string, object>? enterParameters = null)
    {
        // Checks to make sure state you want to change to exist.
        if (!_states.TryGetValue(name, out var newState))
            throw new InvalidOperationException($"State '{name}' does not exist in the StateMachine.");

        // Exits current state.
        _currentState?.Exit();

        // Makes change to provided state.
        _currentState = newState;
        CurrentStateName = name;

        // Enters new state.
        _currentState.Enter(enterParameters);
    }

    /// <summary>
    /// Updates the state if needed.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Update(GameTime gameTime)
    {
        _currentState?.Update(gameTime);
    }

    /// <summary>
    /// Draws the state if needed.
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(GameTime gameTime)
    {
        _currentState?.Draw(gameTime);
    }
    #endregion Methods
}