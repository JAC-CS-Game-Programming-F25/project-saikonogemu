using System;
using System.Collections.Generic;
using CoreLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Pleasing;

#nullable enable

namespace Game.Scripts.Entities.Dice.States;

/// <summary>
/// Represents the state of the player in living form as they are phasing.
/// </summary>
public class PlayerPhaseState : PlayerLivingState
{
    #region Constants
    private const float ACTIVATION_DELAY = 50f;
    private const float ABILITY_DURATION = 2000f;
    private const float COOLDOWN = 2f;
    #endregion Constants

    #region Backing Fields
    private float _abilityCooldownTimer = 0f;
    #endregion Backing Fields

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);

        if (Dice is null) 
            throw new ArgumentNullException("Dice cannot be null in PlayerPhaseState.");

        // Updates the textures.
        Dice.UpdateTexture("Images/Atlas/player_ascension_dice_atlas.xml");
        Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}" + Dice.GetAnimationTypeWithoutDice());

        // Don't want to accidentally cancer the iframes.
        Dice.DiceOpacity = 0.99f;

        Dice.TweenOpacity(ACTIVATION_DELAY, 0f);

        Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/phase"));
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
        if (Dice!.DiceOpacity == 0f)
            StartIFrames();
        else if (Dice!.DiceOpacity == 1f)
            HandleAbilityCooldown(gameTime);

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

    #region Methods
    /// <summary>
    /// Begins the immortality frames for the dice.
    /// </summary>
    private void StartIFrames()
    {
        (Dice as PlayerDice)!.IsPhasing = true;
        Dice!.TweenOpacity(ABILITY_DURATION, 1f);
    }

    /// <summary>
    /// Handles the ability cooldown.
    /// </summary>
    /// <param name="gameTime">The gameTime of the Game.</param>
    private void HandleAbilityCooldown(GameTime gameTime)
    {
        (Dice as PlayerDice)!.IsPhasing = false;
        _abilityCooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_abilityCooldownTimer >= COOLDOWN)
        {
            _abilityCooldownTimer = 0;
            ChangeToNeutral();
        }
    }

    /// <summary>
    /// Switches the state to the neutral state.
    /// </summary>
    private void ChangeToNeutral()
    {
        Dice?.ChangeState("PlayerNeutralState");
    }
    #endregion Methods
}