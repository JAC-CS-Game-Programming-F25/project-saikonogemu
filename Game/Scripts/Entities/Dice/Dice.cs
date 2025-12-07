/***************************************************************
 * File: Dice.cs
 * Author: FarLostBrand
 * Date: December 4, 2025
 * 
 * Summary:
 *  The Dice class represents the main type of entity in the
 *  game. It extends the base GameEntity class and inherits all 
 *  functionality related to animation, physics, and state 
 *  management. Dice specific configuration values may be supplied
 *  through the diceDefinition dictionary during construction.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

public class Dice : GameEntity
{
    #region Properties
    public const float NORMAL_OFFSET = -5.0f;
    public const float DIAGONAL_OFFSET = -10.0f;
    public DiceDirections DiceDirection {get; set;} = DiceDirections.Right;
    #endregion Properties

    #region Constructors
    /// <summary>
    /// Creates a new dice entity instance.
    /// This serves as a generic for the dice to come.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="diceDefinition">All the dice specific parameters.</param>
    public Dice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : base(content, diceDefinition) { }
    #endregion Constructors

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        CurrentAnimation.Scale = Scale;
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
    /// Called when the dice loses a life.
    /// </summary>
    public void LoseLife()
    {
        Health--;

        if (Health <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// Called when the dice hits 0 hp.
    /// </summary>
    public void Death()
    {
        // FIXME: Move to DiceDyingState.
        IsDead = true;

        // We want it to run once.
        UpdateAnimation(GetDiceTypeTexture() + $"_death_animation", 1);
    }

    /// <summary>
    /// Gets the name preset of file paths for that dice.
    /// </summary>
    /// <returns>Returns the name preset.</returns>
    public string GetDiceTypeTexture()
    {
        // Get the end path of the file.
        // e.g. player_dice_atlas.xml
        Match? match = Regex.Match(EntityTexture.FileName, @"([^/]+)_atlas\.xml$");

        // If there was a match.
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return "FailedFileName";
    }
    #endregion Methods
}