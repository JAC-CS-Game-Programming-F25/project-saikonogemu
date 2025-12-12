using CoreLibrary;
using CoreLibrary.Scenes;
using Game.Scripts.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game.Scripts.Scenes.GameSceneItems;
using Microsoft.Xna.Framework.Audio;
using Gum.Forms.Controls;
using System;
using MonoGameGum;

namespace Game.Scripts.Scenes.TitleSceneItems;

public class TitleScene : Scene
{
    #region Backing Fields
    private const string TITLE_TEXT = "Die";
    private const string SUBTITLE_TEXT = "The Rolling Dice Game";

    // The font to use to render normal text.
    private SpriteFont _font;

    // The font used to render the title text.
    private SpriteFont _titleFont;

    // The position to draw the title text at.
    private Vector2 _titleTextPosition;

    // The origin to set for the title text.
    private Vector2 _titleTextOrigin;

    // The position to draw the subtitle text at.
    private Vector2 _subtitleTextPos;

    // The origin to set for the subtitle text.
    private Vector2 _subtitleTextOrigin;

    // The texture used for the background pattern.
    private Texture2D _backgroundPattern;

    // The destination rectangle for the background pattern to fill.
    private Rectangle _backgroundDestination;

    // The offset to apply when drawing the background pattern so it appears to
    // be scrolling.
    private Vector2 _backgroundOffset;

    // The speed that the background pattern scrolls.
    private float _scrollSpeed = 50.0f;

    // The buttons for the title screen panel.
    private Panel _titleScreenButtonsPanel;

    private Button _optionsButton;
    #endregion Backing Fields

    #region Lifecycle Methods

    /// <summary>
    /// The very first method ran when the scene is started.
    /// It's like an entering method. 
    /// </summary>
    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // While on the title screen, we can enable exit on escape so the player
        // can close the game by pressing the escape key.
        Core.ExitOnEscape = true;

        // Set the position and origin for the Title text.
        Vector2 size = _titleFont.MeasureString(TITLE_TEXT);
        _titleTextPosition = new Vector2(640, 100);
        _titleTextOrigin = size * 0.5f;

        // Set the position and origin for the Subtitle text.
        size = _titleFont.MeasureString(SUBTITLE_TEXT);
        _subtitleTextPos = new Vector2(757, 207);
        _subtitleTextOrigin = size * 0.5f;

        // Initialize the offset of the background pattern at zero.
        _backgroundOffset = Vector2.Zero;

        // Set the background pattern destination rectangle to fill the entire
        // screen background.
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

        InitializeUI();
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
        Core.SettingsManager.CreateOptionsPanel();
    }


    /// <summary>
    /// LoadContent comes right before the Initialize method.
    /// It's where all the game assets are set up.
    /// </summary>
    public override void LoadContent()
    {
        base.LoadContent();

        // Load the font for the standard text.
        _font = Core.Content.Load<SpriteFont>("Fonts/peaberrybase_font");

        // Load the font for the title text.
        // FIXME: Change to Die Font
        _titleFont = Content.Load<SpriteFont>("Fonts/peaberrybase_font");

        // Load the background pattern texture.
        _backgroundPattern = Content.Load<Texture2D>("Images/Tiles/level_tiles");

        Core.UISoundEffect = Core.Content.Load<SoundEffect>("Audio/SFX/click");
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinishedExiting)
            Core.ChangeScene(new GameScene(LevelType.Level1));

        // Update the offsets for the background pattern wrapping so that it
        // scrolls down and to the right.
        float offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffset.X -= offset;
        _backgroundOffset.Y -= offset;

        // Ensure that the offsets do not go beyond the texture bounds so it is
        // a seamless wrap.
        _backgroundOffset.X %= _backgroundPattern.Width;
        _backgroundOffset.Y %= _backgroundPattern.Height;

        GumService.Default.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        // Draw the background pattern first using the PointWrap sampler state.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
        Core.SpriteBatch.End();

        if (_titleScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Black * 0.5f;

            // Draw the Title text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_titleFont, TITLE_TEXT, _titleTextPosition + new Vector2(10, 10), dropShadowColor, 0.0f, _titleTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Draw the Title text on top of that at its original position
            Core.SpriteBatch.DrawString(_titleFont, TITLE_TEXT, _titleTextPosition, Color.White, 0.0f, _titleTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Draw the Subtitle text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_titleFont, SUBTITLE_TEXT, _subtitleTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _subtitleTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Draw the Subtitle text on top of that at its original position
            Core.SpriteBatch.DrawString(_titleFont, SUBTITLE_TEXT, _subtitleTextPos, Color.White, 0.0f, _subtitleTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // Always end the sprite batch when finished.
            Core.SpriteBatch.End();
        }

        GumService.Default.Draw();

        AfterDraw(gameTime);
    }
    #endregion Lifecycle Methods

    #region Methods
    /// <summary>
    /// Used to create the title panel and it's contents.
    /// </summary>
    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons.
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();

        // All start button things.
        var startButton = new Button();
        startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        startButton.Visual.X = 50;
        startButton.Visual.Y = -12;
        startButton.Visual.Width = 70;
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);

        // All options button things.
        _optionsButton = new Button();
        _optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsButton.Visual.X = -50;
        _optionsButton.Visual.Y = -12;
        _optionsButton.Visual.Width = 70;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        startButton.IsFocused = true;
    }

    /// <summary>
    /// This is called when the start button is clicked.
    /// </summary>
    /// <param name="sender">Who called this.</param>
    /// <param name="e">Extra event info. This is kind of like WPF.</param>
    private void HandleStartClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);

        // Change to the game scene to start the game.
        ExitScene();
    }

    /// <summary>
    /// This is called when the options button is clicked.
    /// </summary>
    /// <param name="sender">Who called this.</param>
    /// <param name="e">Extra event info. This is kind of like WPF.</param>
    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);
        
        // Set the title panel to be invisible.
        _titleScreenButtonsPanel.IsVisible = false;

        Core.SettingsManager.ActivateOptionsPanel(_titleScreenButtonsPanel, _optionsButton);
    }
    #endregion Methods
}
