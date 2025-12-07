/***************************************************************
 * File: Rigidbody.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  Represents a 2D physics body within the physics system. The
 *  Rigidbody tracks position, velocity, and collision data, and
 *  determines whether it is dynamic (affected by forces) or static.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using Microsoft.Xna.Framework;

namespace CoreLibrary.Physics;

/// <summary>
/// Represents a 2D physics body that can participate in collisions
/// and respond to velocity-based movement.
/// </summary>
public class Rigidbody
{
    #region Fields

    /// <summary>
    /// Indicates whether the rigidbody is dynamic and can move or be affected by forces.
    /// </summary>
    public bool Dynamic;

    /// <summary>
    /// Defines the collider boundaries of this rigidbody.
    /// </summary>
    public RectangleFloat Collider;

    /// <summary>
    /// Represents the current velocity of the rigidbody, used to update its position over time.
    /// </summary>
    public Vector2 Velocity = Vector2.Zero;

    /// <summary>
    /// The normal vector of the last collision this rigidbody resolved.
    /// </summary>
    public Vector2 LastCollisionNormal = Vector2.Zero;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets or sets the position of the rigidbody in world space.
    /// If the rigidbody is not dynamic, the position cannot be changed.
    /// </summary>
    public Vector2 Position
    {
        get => Collider.Position;
        set
        {
            if (!Dynamic) return;
            Collider.Position = value;
        }
    }

    #endregion Properties

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="Rigidbody"/> class.
    /// </summary>
    /// <param name="dynamic">Specifies whether the rigidbody is dynamic and can move.</param>
    /// <param name="position">The initial position of the rigidbody.</param>
    /// <param name="size">The size of the rigidbody collider.</param>
    public Rigidbody(bool dynamic, Vector2 position, Vector2 size)
    {
        Collider = new RectangleFloat(position, size.X, size.Y);
        Dynamic = dynamic;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Rigidbody"/> class.
    /// </summary>
    /// <param name="dynamic">Specifies whether the rigidbody is dynamic and can move.</param>
    /// <param name="position">The initial position of the rigidbody.</param>
    /// <param name="width">The width of the rigidbody collider.</param>
    /// <param name="height">The height of the rigidbody collider.</param>
    public Rigidbody(bool dynamic, Vector2 position, float width, float height)
    {
        Collider = new RectangleFloat(position, width, height);
        Dynamic = dynamic;
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Updates the rigidbody’s position based on its velocity and the elapsed game time.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Update(GameTime gameTime)
    {
        // Exit early if the rigidbody is static.
        if (!Dynamic) return;

        // Calculate delta time.
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Apply velocity to position.
        Position += Velocity * deltaTime;
    }

    #endregion Public Methods
}
