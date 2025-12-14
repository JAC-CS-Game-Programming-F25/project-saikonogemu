using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.UI;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice;
using Game.Scripts.Scenes.TitleSceneItems;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.Managers;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
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
    private Panel? _pausePanel;
    private AnimatedButton? _resumeButton;
    private AnimatedButton? _optionsButton;

    private SoundEffect? _uiSoundEffect;
    private SoundEffect? _panelOpenSong;
    private TextureAtlas? _atlas;

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
        _panelOpenSong = Core.Content.Load<SoundEffect>("Audio/SFX/menu");
        _uiSoundEffect = Core.Content.Load<SoundEffect>("Audio/SFX/click");
        _atlas = TextureAtlas.FromFile(Core.Content, "Images/Atlas/ui_atlas.xml");
        Core.Instance.IsMouseVisible = true;

        if (_dice is null || _gameScene is null)
        {
            throw new ArgumentNullException("Passed dice or gamescene in PlayState shouldn't be null.");
        }

        Fade(PAUSE_FADE_DURATION, true);
        InitializeUI();
        _pausePanel!.IsVisible = true;
        _resumeButton!.IsFocused = true;

        Core.Audio.PlaySoundEffect(_panelOpenSong);
    }

    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreatePausePanel();
        Core.SettingsManager.CreateOptionsPanel(_atlas);
    }

    /// <summary>
    /// Called when exiting this State.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
        
        Core.Instance.IsMouseVisible = false;

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
            return;
        }
        GumService.Default.Update(gameTime);
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _gameScene!.DrawScreenCover(CurrentPauseOpacity);
        Core.SpriteBatch.End();

        // Draw after the shading :D
        GumService.Default.Draw();
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
        {
            Core.Audio.PlaySoundEffect(_uiSoundEffect);
            StartResume(); 
        }
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

    #region UI Methods
    private void CreatePausePanel()
    {
        _pausePanel = new Panel();
        _pausePanel.Anchor(Anchor.Center);
        _pausePanel.Visual.WidthUnits = DimensionUnitType.Absolute;
        _pausePanel.Visual.HeightUnits = DimensionUnitType.Absolute;
        _pausePanel.Visual.Height = 200;
        _pausePanel.Visual.Width = 150;
        _pausePanel.IsVisible = false;
        _pausePanel.AddToRoot();

        TextureRegion backgroundRegion = _atlas!.GetRegion("panel_background");

        NineSliceRuntime background = new NineSliceRuntime();
        background.Dock(Dock.Fill);
        background.Texture = backgroundRegion.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureWidth = backgroundRegion.Width;
        _pausePanel.AddChild(background);

        TextRuntime textInstance = new TextRuntime();
        textInstance.Text = "PAUSED";
        textInstance.CustomFontFile = @"fonts/WhitePeaberry.fnt";
        textInstance.UseCustomFont = true;
        textInstance.FontScale = 1f;
        textInstance.X = 10f;
        textInstance.Y = 20f;
        textInstance.Anchor(Anchor.Top);
        _pausePanel.AddChild(textInstance);

        float buttonSpacing = 50f;

        _resumeButton = new AnimatedButton(_atlas);
        _resumeButton.Text = "RESUME";
        _resumeButton.Anchor(Anchor.Center);
        _resumeButton.Visual.X = 0f;
        _resumeButton.Visual.Y = -buttonSpacing;
        _resumeButton.Click += HandleResumeButtonClicked;
        _pausePanel.AddChild(_resumeButton);

        _optionsButton = new AnimatedButton(_atlas);
        _optionsButton.Text = "OPTIONS";
        _optionsButton.Anchor(Anchor.Center);
        _optionsButton.Visual.X = 0f;
        _optionsButton.Visual.Y = 0f;
        _optionsButton.Click += HandleOptionsClicked;
        _pausePanel.AddChild(_optionsButton);

        AnimatedButton quitButton = new AnimatedButton(_atlas);
        quitButton.Text = "QUIT";
        quitButton.Anchor(Anchor.Center);
        quitButton.Visual.X = 0f;
        quitButton.Visual.Y = buttonSpacing;
        quitButton.Click += HandleQuitButtonClicked;
        _pausePanel.AddChild(quitButton);
    }

    /// <summary>
    /// This is called when the resume button is clicked.
    /// </summary>
    /// <param name="sender">Who called this.</param>
    /// <param name="e">Extra event info. This is kind of like WPF.</param>
    private void HandleResumeButtonClicked(object? sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Make the pause panel invisible to resume the game.
        _pausePanel!.IsVisible = false;
        StartResume();
    }

    /// <summary>
    /// This is called when the quit button is clicked.
    /// </summary>
    /// <param name="sender">Who called this.</param>
    /// <param name="e">Extra event info. This is kind of like WPF.</param>
    private void HandleQuitButtonClicked(object? sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Go back to the title scene.
        Core.ChangeScene(new TitleScene());
    }

    /// <summary>
    /// This is called when the options button is clicked.
    /// </summary>
    /// <param name="sender">Who called this.</param>
    /// <param name="e">Extra event info. This is kind of like WPF.</param>
    private void HandleOptionsClicked(object? sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);
        
        // Set the title panel to be invisible.
        _pausePanel!.IsVisible = false;

        Core.SettingsManager.ActivateOptionsPanel(_pausePanel, _optionsButton);
    }
    #endregion UI Methods
}