using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice;
using Microsoft.Xna.Framework;

namespace Game.Scripts.Scenes.GameScene.States;

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
}

