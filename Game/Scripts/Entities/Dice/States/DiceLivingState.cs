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
}