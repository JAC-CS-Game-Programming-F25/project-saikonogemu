using CoreLibrary;
using CoreLibrary.Scenes;
using Game.Scripts.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game.Scripts.Scenes.GameSceneItems;
using Microsoft.Xna.Framework.Audio;
using Gum.Forms;
using Gum.Forms.Controls;
using System;
using MonoGameGum;
using CoreLibrary.Graphics;
using CoreLibrary.UI;
using Game.Scripts.Scenes.TitleSceneItems;
using CoreLibrary.Physics;

namespace Game.Scripts.Scenes.EndingScenes;

public class VictoryScene : Scene
{
    #region Backing Fields
    const float BUTTON_HEIGHT = 10f;
    const float BUTTON_WIDTH = 70f;
    const float BUTTON_SPACING = 40f;
    private const float VICTORY_OFFSET_MULTIPLIER = 0.45f;
    private const float SUBTITLE_OFFSET_MULTIPLIER = 1.25f;
    private const float VICTORY_SCALE = 4f;
    private const float SHADOW_OFFSET = 5f;
    private const float DICE_SCALE = 7f;
    private const string VICTORY_TEXT = "You Survived";
    private const string SUBTITLE_TEXT = "Dietopia is Freed";

    // The position to draw the subtitle text at.
    private Vector2 _subtitleTextPosition;

    // The origin to set for the subtitle text.
    private Vector2 _subtitleTextOrigin;

    // The dice shown.
    private Sprite _dice;

    // The font used to render the victory text.
    private SpriteFont _victoryFont;

    // The position to draw the victory text at.
    private Vector2 _victoryTextPosition;

    // The origin to set for the victory text.
    private Vector2 _victoryTextOrigin;

    // The buttons for the victory screen panel.
    private Panel _victoryScreenButtonsPanel;

    // The destination of the button click.
    private string _destinationScreenName;

    // The atlas for the buttons.
    private TextureAtlas _atlas;

    // The atlas for the dice.
    private TextureAtlas _diceAtlas;

    private int _playerHealth;
    #endregion Backing Fields

    #region Constructor
    public VictoryScene(int playerHealth)
    {
        _playerHealth = playerHealth;
    }
    #endregion Constructor

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

        // Dimensions.
        int screenWidth  = Core.GraphicsDevice.Viewport.Width;
        int screenHeight = Core.GraphicsDevice.Viewport.Height;

        // Victory.
        Vector2 victorySize = _victoryFont.MeasureString(VICTORY_TEXT);
        _victoryTextPosition = new Vector2(screenWidth / 2f, screenHeight / 2f * VICTORY_OFFSET_MULTIPLIER);
        _victoryTextOrigin = victorySize / 2f;

        // Subtitle.
        Vector2 subtitleSize = _victoryFont.MeasureString(SUBTITLE_TEXT);
        _subtitleTextPosition = new Vector2(screenWidth / 2f, (_victoryTextPosition.Y + victorySize.Y) * SUBTITLE_OFFSET_MULTIPLIER);
        _subtitleTextOrigin = subtitleSize / 2f;

        _dice = _diceAtlas.CreateSprite($"player_ascension_dice_dot{_playerHealth}_horizontal_frame1");
        _dice.CenterOrigin();
        _dice.Scale = new Vector2(DICE_SCALE, DICE_SCALE);

        InitializeUI();
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
    }


    /// <summary>
    /// LoadContent comes right before the Initialize method.
    /// It's where all the game assets are set up.
    /// </summary>
    public override void LoadContent()
    {
        base.LoadContent();

        // Load the font for the title text.
        _victoryFont = Content.Load<SpriteFont>("Fonts/die_font");

        Core.UISoundEffect = Core.Content.Load<SoundEffect>("Audio/SFX/click");

        _atlas = TextureAtlas.FromFile(Core.Content, "Images/Atlas/ui_atlas.xml");

        _diceAtlas = TextureAtlas.FromFile(Core.Content, "Images/Atlas/player_ascension_dice_atlas.xml");
    }

    public override void Update(GameTime gameTime)
    {
        if (IsFinishedExiting)
            switch (_destinationScreenName)
            {
                case "GameScene":
                    Core.ChangeScene(new GameScene(LevelType.Level1));
                break;

                case "TitleScene":
                    Core.ChangeScene(new TitleScene());
                break;
            }

        GumService.Default.Update(gameTime);

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(25, 25, 25, 255));

        // Dimensions.
        int screenWidth  = Core.GraphicsDevice.Viewport.Width;
        int screenHeight = Core.GraphicsDevice.Viewport.Height;

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _dice.Draw(Core.SpriteBatch, new Vector2(screenWidth / 2f, screenHeight / 2f));
        Core.SpriteBatch.End();

        if (_victoryScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.LightGoldenrodYellow * 0.5f;

            // Draw the Title text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_victoryFont, VICTORY_TEXT, _victoryTextPosition + new Vector2(SHADOW_OFFSET, SHADOW_OFFSET), dropShadowColor, 0.0f, _victoryTextOrigin, VICTORY_SCALE, SpriteEffects.None, 1.0f);

            // Draw the Title text on top of that at its original position
            Core.SpriteBatch.DrawString(_victoryFont, VICTORY_TEXT, _victoryTextPosition, Color.White, 0.0f, _victoryTextOrigin, VICTORY_SCALE, SpriteEffects.None, 1.0f);

            // Draw the Subtitle text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_victoryFont, SUBTITLE_TEXT, _subtitleTextPosition + new Vector2(SHADOW_OFFSET, SHADOW_OFFSET), dropShadowColor, 0.0f, _subtitleTextOrigin, VICTORY_SCALE - 2, SpriteEffects.None, 1.0f);

            // Draw the Subtitle text on top of that at its original position
            Core.SpriteBatch.DrawString(_victoryFont, SUBTITLE_TEXT, _subtitleTextPosition, Color.White, 0.0f, _subtitleTextOrigin, VICTORY_SCALE - 2, SpriteEffects.None, 1.0f);

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
        _victoryScreenButtonsPanel = new Panel();
        _victoryScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _victoryScreenButtonsPanel.AddToRoot();

        // All quit button things.
        AnimatedButton quitButton = new AnimatedButton(_atlas);
        quitButton.Anchor(Gum.Wireframe.Anchor.Bottom);
        quitButton.Visual.X = 0;
        quitButton.Visual.Y = -BUTTON_SPACING;
        quitButton.Visual.Height = BUTTON_HEIGHT;
        quitButton.Visual.Width = BUTTON_WIDTH;
        quitButton.Text = "Quit";
        quitButton.Click += HandleQuitClicked;

        // All restart button things.
        AnimatedButton restartButton = new AnimatedButton(_atlas);
        restartButton.Anchor(Gum.Wireframe.Anchor.Bottom);
        restartButton.Visual.X = 0;
        restartButton.Visual.Y = - (BUTTON_HEIGHT + BUTTON_SPACING - quitButton.Visual.Y);
        restartButton.Visual.Height = BUTTON_HEIGHT;
        restartButton.Visual.Width = BUTTON_WIDTH;
        restartButton.Text = "Restart";
        restartButton.Click += HandleRestartClicked;

        _victoryScreenButtonsPanel.AddChild(restartButton);
        _victoryScreenButtonsPanel.AddChild(quitButton);

        restartButton.IsFocused = true;
    }

    /// <summary>
    /// This is called when the restart button is clicked.
    /// </summary>
    /// <param name="sender">Who called this.</param>
    /// <param name="e">Extra event info. This is kind of like WPF.</param>
    private void HandleRestartClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);

        _destinationScreenName = "GameScene";

        // Change to the game scene to restart the game.
        ExitScene();
    }

    /// <summary>
    /// This is called when the quit button is clicked.
    /// </summary>
    /// <param name="sender">Who called this.</param>
    /// <param name="e">Extra event info. This is kind of like WPF.</param>
    private void HandleQuitClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect.
        Core.Audio.PlaySoundEffect(Core.UISoundEffect);
        
        _destinationScreenName = "TitleScene";

        // Change to the game scene to title screen.
        ExitScene();
    }
    #endregion Methods
}