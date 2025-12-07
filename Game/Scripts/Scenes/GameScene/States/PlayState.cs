using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;

namespace DieTheRollingDiceGame;

#nullable enable

/// <summary>
/// Represents the state of an entity, object, etc.
/// </summary>
public class PlayState : State
{
    #region Fields
    private List<Dice>? _dice;
    #endregion Fields

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter();

        _dice = Utils.GetValue(parameters, "dice", new List<Dice>());

        if (_dice is null)
        {
            throw new ArgumentNullException("Passed dice in PlayState shouldn't be null.");
        }
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

        // TODO: PlayState: Updates NPC movements. Keyboard inputs. Gamepad inputs. Update physics manager.
        foreach (Dice die in _dice)
        {
            die.Update(gameTime);
        }
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
    #endregion Lifecycle Methods
}

