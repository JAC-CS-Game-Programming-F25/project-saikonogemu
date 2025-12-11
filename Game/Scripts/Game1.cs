using CoreLibrary;
using Game.Scripts.Levels;
using Game.Scripts.Scenes.GameScene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game.Scripts;

public class Game1 : Core
{
#region Constructors

    public Game1() : base("DieTheRollingDiceGame", 512, 256, true) { }

    #endregion Constructors

    #region Game Lifecycle

    protected override void Initialize()
    {
        // Runs Initialize in Core class, then run LoadContent
        base.Initialize();

        // Init the camera.
        Camera camera = new();

        // Start the game with the title scene.
        // TODO:  Change to be title screen.
        ChangeScene(new GameScene(LevelType.Level1));
    }

    protected override void LoadContent() { }

    #endregion Game Lifecycle
}
