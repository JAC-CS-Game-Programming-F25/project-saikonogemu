/***************************************************************
 * File: Player.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Player class represents the main controllable entity in the
 *  game. It extends the base GameEntity class and inherits all 
 *  functionality related to animation, physics, and state 
 *  management. Player specific configuration values may be supplied
 *  through the playerDefinition dictionary during construction.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

#nullable enable

public class Player : GameEntity
{
    #region Constructors
    /// <summary>
    /// Creates a new player entity instance.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="playerDefinition">All the player specific parameters.</param>
    public Player(ContentManager content, Dictionary<string, object>? playerDefinition = null) : base(content, playerDefinition) {}
    #endregion Constructors
}