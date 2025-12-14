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

    /// <summary>
    /// Clones the Rigidbody instance (deep clone).
    /// </summary>
    /// <returns>Returns the cloned item.</returns>
    public Rigidbody Clone()
    {
        Rigidbody clone = (Rigidbody)MemberwiseClone();
        clone.Collider = Collider.Clone();
        return clone;
    }

    /// <summary>
    /// Cancels the velocity along the normal.
    /// </summary>
    public void CancelVelocityAlongNormal()
    {
        if (LastCollisionNormal == Vector2.Zero)
            return;

        float dotProduct = Vector2.Dot(Velocity, LastCollisionNormal);

        // Makes it so velocity is not destroyed when on walls and such.
        if (dotProduct < 0f)
        {
            Velocity -= dotProduct * LastCollisionNormal;
        }
    }

    /// <summary>
    /// Resolves AABB collision between 2 entities.
    /// </summary>
    /// <param name="a">The first entity's rigidbody.</param>
    /// <param name="b">The second entity's rigidbody.</param>
    /// <returns>Whether they intersected.</returns>
    public static bool ResolveAABBCollision(Rigidbody a, Rigidbody b)
    {
        RectangleFloat A = a.Collider;
        RectangleFloat B = b.Collider;

        if (!a.Collider.Intersects(b.Collider))
        {
            a.LastCollisionNormal = Vector2.Zero;
            b.LastCollisionNormal = Vector2.Zero;
            return false;
        }

        // Calculate overlap on both axes.
        float overlapLeft = A.Right - B.Left;
        float overlapRight = B.Right - A.Left;
        float overlapTop = A.Bottom - B.Top;
        float overlapBottom = B.Bottom - A.Top;

        const float POSITION_BUFFER = 0.01f;

        float minX = Math.Min(overlapLeft, overlapRight);
        float minY = Math.Min(overlapTop, overlapBottom);

        float penetration = Math.Min(minX, minY);

        // If penetration is tiny, ignore it
        if (penetration <= POSITION_BUFFER)
        {
            a.LastCollisionNormal = Vector2.Zero;
            b.LastCollisionNormal = Vector2.Zero;
            return false;
        }

        Vector2 separation;
        Vector2 normal;

        float correction = penetration;

        if (Math.Abs(a.Velocity.X - b.Velocity.X) > Math.Abs(a.Velocity.Y - b.Velocity.Y))
        {
            float push = overlapLeft < overlapRight ? -correction : correction;
            separation = new Vector2(push, 0);
            normal = new Vector2(Math.Sign(push), 0);
        }
        else
        {
            float push = overlapTop < overlapBottom ? -correction : correction;
            separation = new Vector2(0, push);
            normal = new Vector2(0, Math.Sign(push));
        }


        // Split separation.
        if (a.Dynamic && b.Dynamic)
        {
            separation *= 0.5f;
            a.Position += separation;
            b.Position -= separation;
        }
        else if (a.Dynamic)
        {
            a.Position += separation;
        }
        else if (b.Dynamic)
        {
            b.Position -= separation;
        }

        a.LastCollisionNormal = normal;
        b.LastCollisionNormal = -normal;

        return true;
    }
    #endregion Public Methods
}
