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

public class GameOverScene : Scene
{
    #region Backing Fields
    const float BUTTON_HEIGHT = 10f;
    const float BUTTON_WIDTH = 70f;
    const float BUTTON_SPACING = 40f;
    private const float GAME_OVER_OFFSET_MULTIPLIER = 0.60f;
    private const float GAME_OVER_SCALE = 4f;
    private const float SHADOW_OFFSET = 5f;
    private const float DICE_SCALE = 7f;
    private const string GAME_OVER_TEXT = "Game Over";

    // The dice shown.
    private Sprite _dice;

    // The font used to render the game over text.
    private SpriteFont _gameOverFont;

    // The position to draw the game over text at.
    private Vector2 _gameOverTextPosition;

    // The origin to set for the game over text.
    private Vector2 _gameOverTextOrigin;

    // The buttons for the game over screen panel.
    private Panel _gameOverScreenButtonsPanel;

    // The destination of the button click.
    private string _destinationScreenName;

    // The atlas for the buttons.
    private TextureAtlas _atlas;

    // The atlas for the dice.
    private TextureAtlas _diceAtlas;
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

        // Dimensions.
        int screenWidth  = Core.GraphicsDevice.Viewport.Width;
        int screenHeight = Core.GraphicsDevice.Viewport.Height;

        // Game over.
        Vector2 titleSize = _gameOverFont.MeasureString(GAME_OVER_TEXT);
        _gameOverTextPosition = new Vector2(screenWidth / 2f, screenHeight / 2f * GAME_OVER_OFFSET_MULTIPLIER);
        _gameOverTextOrigin = titleSize / 2f;

        _dice = _diceAtlas.CreateSprite("enemy_dice_dot1_horizontal_frame1");
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
        _gameOverFont = Content.Load<SpriteFont>("Fonts/die_font");

        Core.UISoundEffect = Core.Content.Load<SoundEffect>("Audio/SFX/click");

        _atlas = TextureAtlas.FromFile(Core.Content, "Images/Atlas/ui_atlas.xml");

        _diceAtlas = TextureAtlas.FromFile(Core.Content, "Images/Atlas/enemy_dice_atlas.xml");
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

        if (_gameOverScreenButtonsPanel.IsVisible)
        {
            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = Color.Red * 0.5f;

            // Draw the Title text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_gameOverFont, GAME_OVER_TEXT, _gameOverTextPosition + new Vector2(SHADOW_OFFSET, SHADOW_OFFSET), dropShadowColor, 0.0f, _gameOverTextOrigin, GAME_OVER_SCALE, SpriteEffects.None, 1.0f);

            // Draw the Title text on top of that at its original position
            Core.SpriteBatch.DrawString(_gameOverFont, GAME_OVER_TEXT, _gameOverTextPosition, Color.White, 0.0f, _gameOverTextOrigin, GAME_OVER_SCALE, SpriteEffects.None, 1.0f);

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
        _gameOverScreenButtonsPanel = new Panel();
        _gameOverScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _gameOverScreenButtonsPanel.AddToRoot();

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

        _gameOverScreenButtonsPanel.AddChild(restartButton);
        _gameOverScreenButtonsPanel.AddChild(quitButton);

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