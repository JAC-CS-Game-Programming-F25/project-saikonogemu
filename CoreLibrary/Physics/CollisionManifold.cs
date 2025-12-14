/***************************************************************
 * File: CollisionManifold.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The CollisionManifold class manages and resolves collisions 
 *  between rigidbodies and tilemaps. It provides logic for 
 *  detecting and correcting overlapping entities within a 
 *  tile-based environment.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using CoreLibrary.Graphics;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework;

/// <summary>
/// Deals with collisions from dynamic rectangles.
/// </summary>
public class CollisionManifold
{
    #region Properties

    /// <summary>
    /// The first rigidbody involved in the collision.
    /// </summary>
    public Rigidbody RigidbodyA;

    /// <summary>
    /// The second rigidbody involved in the collision.
    /// </summary>
    public Rigidbody RigidbodyB;

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CollisionManifold"/> class.
    /// </summary>
    /// <param name="rigidbodyA">The first rigidbody involved in the collision.</param>
    /// <param name="rigidbodyB">The second rigidbody involved in the collision, if there is one.</param>
    public CollisionManifold(Rigidbody rigidbodyA, Rigidbody rigidbodyB = null)
    {
        RigidbodyA = rigidbodyA;
        RigidbodyB = rigidbodyB;
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Handles tile collisions by checking for intersections
    /// between the entity’s collider and all collidable tiles.
    /// </summary>
    /// <param name="tilemapColliders">The tilemap colliders of the game.</param>
    public void HandleTileCollisions(IEnumerable<RectangleFloat> tilemapColliders)
    {
        foreach (RectangleFloat tile in tilemapColliders)
        {
            // If the entity’s collider intersects with a tile collider
            if (RigidbodyA.Collider.Intersects(tile))
            {
                // Resolve the overlap between the entity and the tile
                ResolveTileCollision(ref RigidbodyA.Collider, tile);
            }
        }
    }

    /// <summary>
    /// Resolves tile collisions by separating the colliding entity
    /// from the tile along the smallest overlap axis.
    /// </summary>
    /// <param name="entity">The entity colliding with the tile.</param>
    /// <param name="tile">The tile it is colliding with.</param>
    private void ResolveTileCollision(ref RectangleFloat entity, RectangleFloat tile)
    {
        // Calculate overlaps on each axis
        float overlapLeft = entity.Right - tile.Left;
        float overlapRight = tile.Right - entity.Left;
        float overlapTop = entity.Bottom - tile.Top;
        float overlapBottom = tile.Bottom - entity.Top;

        // Find the smallest overlap distances
        float minOverlapX = Math.Min(overlapLeft, overlapRight);
        float minOverlapY = Math.Min(overlapTop, overlapBottom);

        // Resolve along the smallest axis to prevent jitter
        if (minOverlapX < minOverlapY)
        {
            if (overlapLeft < overlapRight)
                entity.Left -= overlapLeft;
            else
                entity.Left += overlapRight;
        }
        else
        {
            if (overlapTop < overlapBottom)
                entity.Top -= overlapTop;
            else
                entity.Top += overlapBottom;
        }
    }
    #endregion Public Methods
}
