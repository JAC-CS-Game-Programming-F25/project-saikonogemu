using CoreLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Die__The_Rolling_Dice_Game;

public class Game1 : Core
{
#region Constructors

    public Game1() : base("DieTheRollingDiceGame", 512, 256, false) { }

    #endregion Constructors

    #region Game Lifecycle

    protected override void Initialize()
    {
        // Runs Initialize in Core class, then run LoadContent
        base.Initialize();

        // Start the game with the title scene.
        //ChangeScene(new GameScene());
    }

    protected override void LoadContent() { }

    #endregion Game Lifecycle
}
