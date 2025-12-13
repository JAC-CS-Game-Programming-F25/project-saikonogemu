using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

#nullable enable

namespace Game.Scripts.Entities.Dice.States;

/// <summary>
/// Represents the state of the dice in dying form.
/// </summary>
public class DiceDyingState : State
{
    #region Properties
    /// <summary>
    /// The player instance used in the states.
    /// </summary>
    protected Dice? Dice { get; private set; }
    #endregion Properties

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);

        Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/death"));

        Dice = Utils.GetValue(parameters, "dice", Dice);

        if (Dice is null)
            throw new ArgumentNullException("dice is null in PlayerLivingState.");

        Dice.IsDying = true;

        // We want it to run once.
        Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_death_animation", 1);
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
        if (Dice!.CurrentAnimation.IsDone())
            Dice.IsDead = true;

        base.Update(gameTime);

        Dice.Hitbox.Velocity = Vector2.Zero;
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