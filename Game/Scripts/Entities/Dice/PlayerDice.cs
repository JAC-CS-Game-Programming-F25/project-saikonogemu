
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

public class PlayerDice : Dice
{
    #region Constructors
    /// <summary>
    /// Creates a new PlayerDice entity instance.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="diceDefinition">All the dice specific parameters.</param>
    public PlayerDice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : base(content, diceDefinition)
    {
        // Adds the player neutral state.
        StateMachine.Add("PlayerNeutralState", new PlayerNeutralState(), new Dictionary<string, object> { ["player"] = this });
    }
    #endregion Constructors

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

    #region Methods
    
    #endregion Methods
}