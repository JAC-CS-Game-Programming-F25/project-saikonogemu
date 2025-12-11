using System.Collections.Generic;
using Game.Scripts.Entities.Dice.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

namespace Game.Scripts.Entities.Dice;

/// <summary>
/// Creates a new EnemyDice entity instance.
/// </summary>
/// <param name="content">The content manager used by the scene to load in content.</param>
/// <param name="diceDefinition">All the dice specific parameters.</param>
public class EnemyDice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : NPCDice(content, diceDefinition)
{
    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
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