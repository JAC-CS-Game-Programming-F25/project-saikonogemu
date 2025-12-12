using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pleasing;

namespace Game.Scripts.Scenes.GameSceneItems.States;

#nullable enable

/// <summary>
/// Represents the state of an entity, object, etc.
/// </summary>
public class PauseState : State
{
    #region Fields

    private const float PAUSE_OPACITY = 0.5f;
    private const float PAUSE_FADE_DURATION = 500f;
    private GameScene? _gameScene;
    private List<Dice>? _dice;
    private bool _isExiting;
    public float CurrentPauseOpacity {get; set;}
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

        Fade(PAUSE_FADE_DURATION, true);
    }

    /// <summary>
    /// Called when exiting this State.
    /// </summary>
    public override void Exit()
    {
        base.Exit();

        foreach (Dice dice in _dice!)
        {
            dice.IsFrozen = false;
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

        if (_isExiting)
        {
            if (CurrentPauseOpacity <= 0)
            {
                _isExiting = false;
                Resume();
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

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _gameScene.DrawScreenCover(CurrentPauseOpacity);
        Core.SpriteBatch.End();
    }
    #endregion Lifecycle Methods

    #region Methods
    /// <summary>
    /// Handles key inputs while in this state.
    /// </summary>
    private void HandleGameKeyInputs()
    {
        // If the ESC key is down, resume game.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
            StartResume(); 
    }

    /// <summary>
    /// Starts to opens Play state.
    /// </summary>
    private void StartResume()
    {
        if (!_isExiting)
        {
            Fade(PAUSE_FADE_DURATION, false);
            _isExiting = true;
        }
    }

    /// <summary>
    /// Opens Play state.
    /// </summary>
    private void Resume()
    {
        _gameScene!.ChangeState("PlayState", new Dictionary<string, object> { ["dice"] = _dice!, ["gameScene"] = _gameScene });
    }

    /// <summary>
    /// Creates a fade effect.
    /// </summary>
    /// <param name="duration">How long the effect is.</param>
    /// <param name="fadeToBlack">Whether to fade to black.</param>
    public void Fade(float duration, bool fadeToBlack)
    {
        // Creates a tweening timeline (makes it so we can repeat/stop it).
        TweenTimeline timeline = Tweening.NewTimeline();

        TweenableProperty<float> fadeProp = timeline.AddFloat(this, nameof(CurrentPauseOpacity));

        float start = CurrentPauseOpacity;
        float end = fadeToBlack ? PAUSE_OPACITY : 0f;

        fadeProp.AddFrame(0f, start);
        fadeProp.AddFrame(duration, end);            
    }
    #endregion Methods
}