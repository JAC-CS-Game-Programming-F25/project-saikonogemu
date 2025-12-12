using System.Collections.Generic;
using Game.Scripts.Entities.Dice.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

namespace Game.Scripts.Entities.Dice;

public class NPCDice: Dice
{
    public const float SPEED = 75f;

    #region Constructors
    /// <summary>
    /// Creates a new TargetDice entity instance.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="diceDefinition">All the dice specific parameters.</param>
    public NPCDice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : base(content, diceDefinition)
    {
        // Adds the player neutral state.
        AddState("NPCLivingState", new NPCLivingState(), new Dictionary<string, object> { ["dice"] = this });

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
}