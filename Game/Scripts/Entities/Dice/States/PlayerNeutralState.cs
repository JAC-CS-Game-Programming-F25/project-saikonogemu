using System;
using System.Collections.Generic;
using CoreLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#nullable enable

namespace Game.Scripts.Entities.Dice.States;

/// <summary>
/// Represents the state of the player in living neutral form.
/// </summary>
public class PlayerNeutralState : PlayerLivingState
{
    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);

        // Updates the textures of the dice.
        Dice!.UpdateTexture("Images/Atlas/player_dice_atlas.xml");
        Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}" + Dice.GetAnimationTypeWithoutDice());
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

        CheckAbilities();
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

    #region Ability Methods
    /// <summary>
    /// Checks to see if an ability is triggered.
    /// </summary>
    private void CheckAbilities()
    {
        // If the Z keys are down, activate the dash.
        if ((Core.Input.Keyboard.IsKeyDown(Keys.Z) || Core.Input.Keyboard.IsKeyDown(Keys.J)) && (Math.Abs(Dice!.Hitbox.Velocity.X) > 0 || Math.Abs(Dice!.Hitbox.Velocity.Y) > 0) && (Dice as PlayerDice)!.CanDash) 
            Dash();

        // If the X keys are down, activate phasing.
        else if ((Core.Input.Keyboard.IsKeyDown(Keys.X) || Core.Input.Keyboard.IsKeyDown(Keys.K)) && (Dice as PlayerDice)!.CanPhase) Phase();
    }

    /// <summary>
    /// Switches the state to the dash state.
    /// </summary>
    private void Dash()
    {
        Dice?.ChangeState("PlayerDashState", new Dictionary<string, object> { ["dice"] = Dice });
    }

    /// <summary>
    /// Switches the state to the phase state.
    /// </summary> 
    private void Phase()
    {
        Dice?.ChangeState("PlayerPhaseState", new Dictionary<string, object> { ["dice"] = Dice });
    }
    #endregion Ability Methods
}