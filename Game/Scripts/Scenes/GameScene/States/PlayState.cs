using CoreLibrary;
using Microsoft.Xna.Framework;

namespace DieTheRollingDiceGame;

/// <summary>
/// Represents the state of an entity, object, etc.
/// </summary>
public class PlayState : State
{
    #region Fields
    
    #endregion Fields

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(object parameters = null)
    {
        base.Enter();
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

        // TODO: PlayState: Updates NPC movements. Keyboard inputs. Gamepad inputs. Update physics manager.
        player.Update(gameTime);
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

