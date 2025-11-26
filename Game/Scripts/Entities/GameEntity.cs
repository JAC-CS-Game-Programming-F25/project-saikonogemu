/***************************************************************
 * File: GameEntity.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The GameEntity class represents the abstract base for all
 *  in-game entities. It provides core functionality such as
 *  texture/animation handling, physics-based hitboxes, health
 *  attributes, and optional state machine integration. Derived
 *  entities define custom behaviors while relying on shared
 *  update, draw, and lifecycle logic.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Physics;
using CoreLibrary.StateMachine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

/// <summary>
/// Represents the base class for all game entities.
/// Provides lifecycle methods for initialization, content management,
/// updating, drawing, and disposal.
/// </summary>
public abstract class GameEntity
{
    #region Properties
    public StateMachine? StateMachine { get; set; }

    /// <summary>
    /// Gets the texture used for all game entities.
    /// </summary>
    public TextureAtlas EntityTexture { get; private set; }

    /// <summary>
    /// Gets the current game entity's texture.
    /// </summary>
    public AnimatedSprite CurrentAnimation { get; private set; }

    /// <summary>
    /// Gets the hitbox of the game entity.
    /// </summary>
    public Rigidbody Hitbox { get; private set; }

    /// <summary>
    /// Gets the total health of the entity.
    /// </summary>
    public int TotalHealth { get; set; }

    /// <summary>
    /// Gets the current health of the entity.
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// Gets the strength of the entity.
    /// </summary>
    public int Strength { get; set; }

    /// <summary>
    /// Gets whether the entity is dead.
    /// </summary>
    public bool IsDead { get; set; } = false;
    #endregion Properties

    #region Constructors

    /// <summary>
    /// Creates a new game entity instance.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="entityDefinition">All the entity specific parameters.</param>
    public GameEntity(ContentManager content, Dictionary<string, object>? entityDefinition = null)
    {
        // Entity Sprite Name.
        string name = GetValue(entityDefinition, "name", "default");

        // Entity Texture.
        EntityTexture = TextureAtlas.FromFile(content, "images/entity-definition.xml");

        // Current Animation.
        CurrentAnimation = EntityTexture.CreateAnimatedSprite(name);

        // Collisions.
        Vector2 sizeOffset = GetValue(entityDefinition, "sizeOffset", Vector2.Zero);
        Vector2 position = GetValue(entityDefinition, "position", Vector2.Zero);
        Vector2 positionOffset = GetValue(entityDefinition, "positionOffset", Vector2.Zero);
        Hitbox = PhysicsManager.Instance.CreateSpriteRigidBody(CurrentAnimation, sizeOffset, position, positionOffset);

        // Physics.
        Hitbox.Velocity = GetValue(entityDefinition, "velocity", Vector2.One);

        // Lively Things.
        TotalHealth = GetValue(entityDefinition, "entityTotalHealth", 1);
        Health = TotalHealth;
        Strength = GetValue(entityDefinition, "entityStrength", 1);
    }

    #endregion Constructors

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public virtual void Update(GameTime gameTime)
    {
        // Updates the StateMachine.
        StateMachine?.Update(gameTime);

        // Updates the animation.
        CurrentAnimation?.Update(gameTime);
    }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public virtual void Draw(GameTime gameTime)
    {
        // Draws the StateMachine.
        StateMachine?.Draw(gameTime);

        // Draws the animation.
        CurrentAnimation?.Draw(Core.SpriteBatch, Hitbox.Position);
    }

    #endregion Update and Draw

    #region Methods
    /// <summary>
    /// Changes the current state to provided state.
    /// </summary>
    /// <param name="name">The name of the state to change to.</param>
    /// <param name="parameters">Optional enter methods parameters.</param>
    public void ChangeState(string name, object? parameters = null)
    {
        StateMachine?.Change(name, parameters);
    }
    #endregion Methods

    #region Helper Methods   
    /// <summary>
    /// Gets the potentially null value from a potentially null dictionary.
    /// </summary>
    /// <typeparam name="T">The type we are trying to convert to.</typeparam>
    /// <param name="dictionary">The potentially null dictionary.</param>
    /// <param name="key">The key to the potentially null or invalid value.</param>
    /// <param name="fallback">The fallback default.</param>
    /// <returns>The fallback value if it fails; the value itself otherwise.</returns>
    T GetValue<T>(Dictionary<string, object>? dictionary, string key, T fallback)
    {
        if (dictionary != null && dictionary.TryGetValue(key, out var value) && value is T castValue)
            return castValue;
        return fallback;
    }
    #endregion Helper Methods
}
