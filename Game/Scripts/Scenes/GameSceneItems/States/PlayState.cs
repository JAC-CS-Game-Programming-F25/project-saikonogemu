using System;
using System.Collections.Generic;
using CoreLibrary;
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

        for (int i = 0; i < _dice!.Count; i ++)
        {
            if (_dice[i].IsDead)
            {
                _dice[i].Delete();
                _dice.RemoveAt(i);
                continue;
            }

            // Check for entity-entity collision for knockback.
            // We make sure to not double check.
            for (int j = i + 1; j < _dice!.Count; j ++)
            {
                if (_dice[i].IsDying)
                    break;
                else if (_dice[j].IsDying)
                    continue;

                // Check and handle collision.
                bool didCollide = _dice[i].DidCollideWithOtherDice(_dice[j]);

                if (didCollide)
                {
                    if (_dice[i] is PlayerDice)
                    {
                        if((_dice[i] as PlayerDice)!.IsPhasing)
                            continue;                       

                        // Perform life loss.
                        if (_dice[j] is EnemyDice)
                            _dice[i].LoseLife();

                        _dice[j].LoseLife();
                    }

                    // Perform knockback.
                    _dice[i].Knockback();
                }
            }

            // Update dice.
            _dice[i].Update(gameTime);
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

