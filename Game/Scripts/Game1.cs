using CoreLibrary;
using Game.Scripts.Levels;
using Game.Scripts.Scenes.TitleSceneItems;
using Gum.Forms;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;

namespace Game.Scripts;

public class Game1 : Core
{
#region Constructors

    public Game1() : base("DieTheRollingDiceGame") { }

    #endregion Constructors

    #region Game Lifecycle

    protected override void Initialize()
    {
        // Runs Initialize in Core class, then run LoadContent
        base.Initialize();

        // Init the camera.
        Camera camera = new();

        // Initialize the Gum UI service
        InitializeGum();

        // Start the game with the title scene.
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent() { }

    #endregion Game Lifecycle

    #region GUM UI
    /// <summary>
    /// Initializes the Gum UI. 
    /// This was taken from the Gum Tutorial.
    /// </summary>
    private void InitializeGum()
    {
        // Initialize the Gum service. The second parameter specifies
        // the version of the default visuals to use. V2 is the latest
        // version.
        GumService.Default.Initialize(this, DefaultVisualsVersion.V2);

        // Tell the Gum service which content manager to use.  We will tell it to
        // use the global content manager from our Core.
        GumService.Default.ContentLoader.XnaContentManager = Content;

        // Register keyboard input for UI control.
        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

        // Register gamepad input for Ui control.
        FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

        // Customize the tab reverse UI navigation to also trigger when the keyboard
        // Up arrow key is pushed.
        FrameworkElement.TabReverseKeyCombos.Add(
        new KeyCombo() { PushedKey = Keys.Up });

        // Customize the tab UI navigation to also trigger when the keyboard
        // Down arrow key is pushed.
        FrameworkElement.TabKeyCombos.Add(
        new KeyCombo() { PushedKey = Keys.Down });

        // The assets created for the UI were done so at 1/4th the size to keep the size of the
        // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
        // the game's resolution then tell gum to zoom in by a factor of 4.
        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
        GumService.Default.Renderer.Camera.Zoom = 4.0f;
    }
    #endregion GUM UI
}
