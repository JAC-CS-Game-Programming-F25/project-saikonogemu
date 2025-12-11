/***************************************************************
 * File: PhysicsManager.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The PhysicsManager class serves as the central physics system
 *  responsible for updating rigidbodies, detecting collisions, 
 *  and resolving interactions with static tile colliders.
 * 
 *  This class implements a singleton pattern for global access 
 *  and manages rigidbodies, collision manifolds, and tile-based
 *  collision checks.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreLibrary.Graphics;
using Microsoft.Xna.Framework;

namespace CoreLibrary.Physics;

/// <summary>
/// Manages physics objects, collision detection, and rigidbody updates.
/// </summary>
public sealed class PhysicsManager
{
    #region Fields

    private readonly List<RectangleFloat> _tileColliders = new();
    private readonly List<Rigidbody> _rigidBodies = new();
    private readonly List<CollisionManifold> _collisionManifolds = new();

    private static readonly Lazy<PhysicsManager> s_instance = 
        new(() => new PhysicsManager());

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets or sets all tile colliders currently used for collision checks.
    /// </summary>
    public IEnumerable<RectangleFloat> TileColliders
    {
        get => _tileColliders;
        set
        {
            _tileColliders.Clear();
            if (value != null)
                _tileColliders.AddRange(value);
        }
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="PhysicsManager"/>.
    /// </summary>
    public static PhysicsManager Instance => s_instance.Value;

    /// <summary>
    /// Gets a read-only collection of all registered rigidbodies.
    /// </summary>
    public ReadOnlyCollection<Rigidbody> RigidBodies => _rigidBodies.AsReadOnly();

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Private constructor for singleton initialization.
    /// </summary>
    private PhysicsManager() { }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Updates all physics objects and handles collision resolution.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Update(GameTime gameTime)
    {
        _collisionManifolds.Clear();

        // Build tile collision manifolds for dynamic bodies.
        foreach (Rigidbody rigidbody in _rigidBodies)
        {
            if (!rigidbody.Dynamic)
                continue;

            _collisionManifolds.Add(new CollisionManifold(rigidbody));
        }

        // Process collisions against static tile colliders.
        foreach (CollisionManifold manifold in _collisionManifolds)
            manifold.HandleTileCollisions(TileColliders);

        // Update all rigidbodies.
        foreach (Rigidbody rigidbody in _rigidBodies)
            rigidbody.Update(gameTime);

        /*
         * NOTE:
         * Entity-to-entity collision resolution will be implemented
         * in a later update. Preliminary logic is retained below for reference.
         */

        // float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        // int count = _rigidBodies.Count;
        // var movedThisFrame = new HashSet<Rigidbody>();
        //
        // for (int i = 0; i < count; i++)
        // {
        //     Rigidbody a = _rigidBodies[i];
        //     for (int j = i + 1; j < count; j++)
        //     {
        //         Rigidbody b = _rigidBodies[j];
        //
        //         if (!a.Dynamic && !b.Dynamic)
        //             continue;
        //
        //         RectangleFloat broadphaseRect = Rigidbody.GetSweptBroadphaseRect(a, b, deltaTime);
        //
        //         if (broadphaseRect.Intersects(b.Collider))
        //             _collisionManifolds.Add(new CollisionManifold(a, b));
        //     }
        // }
        //
        // foreach (var manifold in _collisionManifolds)
        //     manifold.ResolveCollision(deltaTime, movedThisFrame);
        //
        // foreach (var rb in _rigidBodies)
        // {
        //     if (!rb.Dynamic || movedThisFrame.Contains(rb))
        //         continue;
        //
        //     rb.Position += rb.Velocity * deltaTime;
        // }
    }

    /// <summary>
    /// Clears all registered rigidbodies from the simulation.
    /// </summary>
    public void Reset() => _rigidBodies.Clear();

    /// <summary>
    /// Adds a rigidbody to the physics simulation.
    /// </summary>
    /// <param name="rigidbody">The rigidbody to register.</param>
    public void AddRigidbody(Rigidbody rigidbody) => _rigidBodies.Add(rigidbody);

    /// <summary>
    /// Removes a rigidbody to the physics simulation.
    /// </summary>
    /// <param name="rigidbody">The rigidbody to remove.</param>
    public void RemoveRigidbody(Rigidbody rigidbody) => _rigidBodies?.Remove(rigidbody);

    /// <summary>
    /// Creates and registers a rigidbody based on a <see cref="Sprite"/>.
    /// </summary>
    /// <param name="sprite">The sprite defining collider size (bounding box).</param>
    /// <param name="sizeOffset">The size offset of the hitbox from the bounding box.</param>
    /// <param name="position">The starting position of the rigidbody.</param>
    /// <param name="positionOffset">The position offset of the hitbox from the bounding box.</param>
    /// <returns>The created rigidbody instance.</returns>
    public Rigidbody CreateSpriteRigidBody(Sprite sprite, Vector2 sizeOffset, Vector2 position, Vector2 scale)
    {
        Vector2 size = new Vector2(sprite.Width + sizeOffset.X, sprite.Height + sizeOffset.Y) * scale;
        Rigidbody rigidbody = new(true, position, size);
        AddRigidbody(rigidbody);
        return rigidbody;
    }

    /// <summary>
    /// Creates and registers a rigidbody based on an <see cref="AnimatedSprite"/>.
    /// </summary>
    /// <param name="animatedSprite">The animated sprite defining collider size.</param>
    /// <param name="sizeOffset">The size offset of the hitbox from the bounding box.</param>
    /// <param name="position">The starting position of the rigidbody.</param>
    /// <param name="positionOffset">The position offset of the hitbox from the bounding box.</param>
    /// <returns>The created rigidbody instance.</returns>
    public Rigidbody CreateSpriteRigidBody(AnimatedSprite animatedSprite, Vector2 sizeOffset, Vector2 position, Vector2 scale)
    {
        Vector2 size = new Vector2(animatedSprite.Width + sizeOffset.X, animatedSprite.Height + sizeOffset.Y) * scale;
        Rigidbody rigidbody = new(true, position, size);
        AddRigidbody(rigidbody);
        return rigidbody;
    }

    #endregion Public Methods
}
