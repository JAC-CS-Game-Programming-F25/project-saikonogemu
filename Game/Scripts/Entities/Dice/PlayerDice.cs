using System.Collections.Generic;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

namespace Game.Scripts.Entities.Dice;

public class PlayerDice : Dice
{
    public const float SPEED = 100f;

    #region Properties
    public bool CanDash {get; private set;}

    public bool CanPhase {get; private set;}

    public bool IsPhasing {get; set;}
    #endregion Properties

    #region Constructors
    /// <summary>
    /// Creates a new PlayerDice entity instance.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="diceDefinition">All the dice specific parameters.</param>
    public PlayerDice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : base(content, diceDefinition)
    {
        CanDash = Utils.GetValue(diceDefinition, "hasDash", false);

        CanPhase = Utils.GetValue(diceDefinition, "hasPhase", false);

        // Adds the player neutral state.
        AddState("PlayerNeutralState", new PlayerNeutralState(), new Dictionary<string, object> { ["dice"] = this });

        // Adds the player dash state.
        AddState("PlayerDashState", new PlayerDashState());

        // Adds the player phase state.
        AddState("PlayerPhaseState", new PlayerPhaseState());

        // Adds the player phase state.
        AddState("DiceDyingState", new DiceDyingState());
    }
    #endregion Constructors

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
        if (IsFrozen)
            return;

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }

    #endregion Update and Draw

    #region Methods
    /// <summary>
    /// Clones the PlayerDice instance as a ghost.
    /// </summary>
    /// <returns>Returns the cloned item.</returns>
    public PlayerDice CreateGhost()
    {
        // Clones the player.
        PlayerDice ghost = (PlayerDice)MemberwiseClone();

        // Copies only what we need.
        ghost.CurrentAnimation = CurrentAnimation.Clone();
        ghost.DiceOpacity = DiceOpacity;

        // Snapshot the position
        ghost.Hitbox = Hitbox.Clone();
        ghost.Hitbox.Velocity = Vector2.Zero;
        ghost.Hitbox.Dynamic = false;

        // Ghost should not update states.
        ghost.StateMachine = null;

        return ghost;
    }

    #endregion Methods
}