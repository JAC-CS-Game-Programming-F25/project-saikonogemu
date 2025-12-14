using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Physics;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Game.Scripts.Scenes.GameSceneItems.States;

#nullable enable

/// <summary>
/// Represents the state of an entity, object, etc.
/// </summary>
public class PlayState : State
{
    #region Fields
    private GameScene? _gameScene;
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

        _gameScene = Utils.GetValue(parameters, "gameScene", _gameScene);
        _dice = Utils.GetValue(parameters, "dice", new List<Dice>());

        if (_dice is null || _gameScene is null)
        {
            throw new ArgumentNullException("Passed dice or gamescene in PlayState shouldn't be null.");
        }
    }

    /// <summary>
    /// Called when exiting this State.
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        foreach (Dice dice in _dice!)
        {
            dice.Hitbox.Velocity = Vector2.Zero;
            dice.IsFrozen = true;
        }
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime">The GameTime of the game.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        HandleGameKeyInputs();

        // Update the dice.
        foreach (var dice in _dice!)
            dice.Update(gameTime);

        // Resolve collisions.
        for (int i = _dice!.Count - 1; i >= 0; i--)
        {
            if (_dice[i].IsDead)
            {
                if (_dice[i] is PlayerDice)
                    _gameScene!.IsPlayerDead = true;
                else
                {
                    _dice[i].Delete();
                    _dice.RemoveAt(i);
                }
                continue;
            }

            // Check for entity-entity collision for knockback.
            // We make sure to not double check.
            for (int j = i - 1; j >= 0; j--)
            {
                Dice a = _dice[i];
                Dice b = _dice[j];

                PlayerDice? player = a as PlayerDice ?? b as PlayerDice;
                Dice? other = player == a ? b : player == b ? a : null;

                if (a.IsDying)
                    break;
                else if (b.IsDying)
                    continue;

                if (a is NPCDice aNPCDice && player is not null)
                {
                    aNPCDice.HandlePlayerVisionCollision(player, gameTime);
                } else if (b is NPCDice bNPCDice && player is not null)
                {
                    bNPCDice.HandlePlayerVisionCollision(player, gameTime);
                }

                bool didCollide = false;

                if (player is null || !player.IsPhasing)
                    didCollide = Rigidbody.ResolveAABBCollision(a.Hitbox, b.Hitbox);

                if (!didCollide)
                    continue;

                // Stop inward motion.
                a.Hitbox.CancelVelocityAlongNormal();
                b.Hitbox.CancelVelocityAlongNormal();

                if (a is NPCDice diceA && b is NPCDice diceB)
                {
                    // Swaps their directions so they aren't annoying.
                    // And adds knockback so they don't perma swap.
                    diceA.HandleNPCCollision();
                    diceB.HandleNPCCollision();
                }

                if (player != null && other != null)
                {

                    if (player.IsHavingKnockback)
                        player.Hitbox.Velocity = Vector2.Zero;

                    if (!player.IsPhasing)
                    {
                        if (other is EnemyDice)
                            player.LoseLife();

                        other.LoseLife();

                        player.Knockback();
                    }
                }
            }
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

    #region Methods
    /// <summary>
    /// Handles key inputs while in this state.
    /// </summary>
    private void HandleGameKeyInputs()
    {
        // If the R key is down, restart game.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.R)) 
            Restart();
        
        else if (Core.Input.Keyboard.WasKeyJustPressed(Keys.F1))
            ToggleDebugMode();

        // If the ESC key is down, pause game.
        else if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
            Pause(); 
    }

    /// <summary>
    /// Restarts game scene.
    /// </summary>
    private void Restart()
    {
        Core.ChangeScene(new GameScene(Levels.LevelType.Level1, 6));
    }

    /// <summary>
    /// Toggles the debug mode for the game.
    /// </summary>
    private void ToggleDebugMode()
    {
        Core.DebugMode = !Core.DebugMode;
    }

    /// <summary>
    /// Opens Pause state.
    /// </summary>
    private void Pause()
    {
        _gameScene!.ChangeState("PauseState", new Dictionary<string, object> { ["dice"] = _dice!, ["gameScene"] = _gameScene });
    }
    #endregion Methods
}

