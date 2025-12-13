using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Pleasing;

#nullable enable

namespace Game.Scripts.Entities.Dice.States;

/// <summary>
/// Represents the state of the player in living form as they are dashing.
/// </summary>
public class PlayerDashState : PlayerLivingState
{
    #region Constants
    const int DashPower = 3;
    const float DECELERATION = 20f; 
    private const float GHOST_SPAWN_INTERVAL = 0.2f;
    private const float GHOST_INITIAL_OPACITY = 1f;
    private const float GHOST_FADE_DURATION = 1000f;
    #endregion Constants

    #region Properties
    private readonly List<PlayerDice> _ghosts = new();
    private float _ghostSpawnTimer = 0f;

    public bool IsDashing {get; private set;}
    #endregion Properties

    #region Lifecycle Methods   
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);

        if (Dice is null) 
            throw new ArgumentNullException("Dice cannot be null in PlayerDashState.");

        // Updates the textures.
        Dice.UpdateTexture("Images/Atlas/player_dash_dice_atlas.xml");
        Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}" + Dice.GetAnimationTypeWithoutDice());

        // Boosts the velocity!
        Dice.Hitbox.Velocity *= DashPower;
        IsDashing = true;

        // Audio.
        int sfxNumber = new Random().Next(0, 3);

        switch (sfxNumber)
        {
            case 0:
                Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/dash1"));
            break;

            case 1:
                Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/dash2"));
            break;

            case 2:
                Core.Audio.PlaySoundEffect(Core.Content.Load<SoundEffect>("Audio/SFX/dash3"));
            break;
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
        // Updates all the ghost.
        for (int i = _ghosts.Count - 1; i >= 0; i--)
        {
            PlayerDice ghost = _ghosts[i];
            ghost.Update(gameTime);

            // Removes all old ghosts.
            if (ghost.DiceOpacity <= 0.01f)
                _ghosts.RemoveAt(i);
        }

        if (IsDashing == true)
        {
            HandleGhostGeneration(gameTime);
            HandleDashDeceleration();
            HandleCancelDash();
            return;
        }

        // If there are no ghosts left, and we are done dashing, we return to the NeutralState.
        if (_ghosts.Count == 0 && IsDashing == false)
            ChangeToNeutral();


        base.Update(gameTime);
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime">The GameTime of the game.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        // Renders all the ghost.
        foreach(PlayerDice ghost in _ghosts)
            ghost.Draw(gameTime);
    }
    #endregion Lifecycle Methods

    #region Dash Methods
    /// <summary>
    /// Checks to see if dash is being canceled. Canceled if it is.
    /// </summary>
    private void HandleCancelDash()
    {
        if (Core.Input.Keyboard.AnyKeyJustPressed)
        {
            Dice!.FlattenSpeed();
            EndDash();
        }
    }

    /// <summary>
    /// Generates ghosts based on a timer.
    /// </summary>
    /// <param name="gameTime">The gameTime of the Game.</param>
    private void HandleGhostGeneration(GameTime gameTime)
    {
        _ghostSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_ghostSpawnTimer >= GHOST_SPAWN_INTERVAL)
        {
            _ghostSpawnTimer = 0f;
            SpawnGhost();
        }
    }

    /// <summary>
    /// Spawns a ghost copy of the player.
    /// </summary>
    private void SpawnGhost()
    {
        // Makes a ghost clone.
        PlayerDice ghost = ((PlayerDice)Dice!).CreateGhost();

        // Starts ghost slightly transparent.
        ghost.DiceOpacity = GHOST_INITIAL_OPACITY;

        // Tweens if out.
        ghost.TweenOpacity(GHOST_FADE_DURATION, 0);

        // Adds it to the list to render.
        _ghosts.Add(ghost);
    }

    /// <summary>
    /// Makes the dash slowly end by adjusting the dice's velocity.
    /// </summary>
    private void HandleDashDeceleration()
    {
        // Progressively get to dice's speed.
        Dice!.ClampAxisTowardsSpeed(ref Dice!.Hitbox.Velocity.X, DECELERATION);
        Dice!.ClampAxisTowardsSpeed(ref Dice!.Hitbox.Velocity.Y, DECELERATION);

        // Check to see if the dash is over.
        if (Math.Abs(Dice!.Hitbox.Velocity.X) <= Dice!.Speed && Math.Abs(Dice!.Hitbox.Velocity.Y) <= Dice!.Speed)
        {
            EndDash();
        }
    }

    /// <summary>
    /// Called when it's time to end the dash.
    /// </summary>
    private void EndDash()
    {
        IsDashing = false;

        // Updates the textures of the dice.
        Dice!.UpdateTexture("Images/Atlas/player_ascension_dice_atlas.xml");
        Dice.UpdateAnimation(Dice.GetDiceTypeTexture() + $"_dot{Dice.Health}" + Dice.GetAnimationTypeWithoutDice());
    }

    /// <summary>
    /// Switches the state to the neutral state.
    /// </summary>
    private void ChangeToNeutral()
    {
        Dice?.ChangeState("PlayerNeutralState");
    }
    #endregion Dash Methods
}