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
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

namespace Game.Scripts.Entities;

/// <summary>
/// Represents the base class for all game entities.
/// Provides lifecycle methods for initialization, content management,
/// updating, drawing, and disposal.
/// </summary>
public abstract class GameEntity
{
    #region Properties
    private ContentManager _contentManager;

    /// <summary>
    /// Gets the statemachine of the entity.
    /// </summary>
    protected StateMachine? StateMachine { get; set; } = new StateMachine();

    /// <summary>
    /// Gets the texture used for all game entities.
    /// </summary>
    public TextureAtlas EntityTexture { get; private set; }

    /// <summary>
    /// Gets the name of the current animation.
    /// </summary>
    public string CurrentAnimationName { get; private set; }

    /// <summary>
    /// Gets the current game entity's texture.
    /// </summary>
    public AnimatedSprite CurrentAnimation { get; protected set; }

    /// <summary>
    /// Gets the hitbox of the game entity.
    /// </summary>
    public Rigidbody Hitbox { get; set; }

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
    /// Gets the scale of the entity.
    /// </summary>
    public Vector2 Scale { get; set; }

    /// <summary>
    /// Gets the speed of the entity.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Gets whether the entity is dying.
    /// </summary>
    public bool IsDying { get; set; }

    /// <summary>
    /// Gets whether the entity is dead.
    /// </summary>
    public bool IsDead { get; set; }
    #endregion Properties

    #region Constructors

    /// <summary>
    /// Creates a new game entity instance.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="entityDefinition">All the entity specific parameters.</param>
    public GameEntity(ContentManager content, Dictionary<string, object>? entityDefinition = null)
    {
        _contentManager = content;

        // Entity Texture.
        EntityTexture = TextureAtlas.FromFile(content, Utils.GetValue(entityDefinition, "texture", "FailedTextureLoading"));

        // Current Animation.
        CurrentAnimationName = Utils.GetValue(entityDefinition, "animationName", "default");
        CurrentAnimation = EntityTexture.CreateAnimatedSprite(CurrentAnimationName);
        Scale = Utils.GetValue(entityDefinition, "scale", Vector2.One);
        CurrentAnimation.Origin = Utils.GetValue(entityDefinition, "positionOffset", Vector2.Zero) * Scale;

        // Collisions.
        Vector2 sizeOffset = Utils.GetValue(entityDefinition, "sizeOffset", Vector2.Zero);
        Vector2 position = Utils.GetValue(entityDefinition, "position", Vector2.Zero);
        Hitbox = PhysicsManager.Instance.CreateSpriteRigidBody(CurrentAnimation, sizeOffset, position, Scale);

        // Physics.
        Hitbox.Velocity = Utils.GetValue(entityDefinition, "velocity", Vector2.Zero);
        Speed = Utils.GetValue(entityDefinition, "speed", 100f) * Scale.X;

        // Lively Things.
        TotalHealth = Utils.GetValue(entityDefinition, "entityTotalHealth", 1);
        Health = TotalHealth;
        Strength = Utils.GetValue(entityDefinition, "entityStrength", 1);
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
    public void AddState(string stateName, State state, Dictionary<string, object>? parameters = null)
    {
        StateMachine?.Add(stateName, state, parameters);
    }

    /// <summary>
    /// Changes the current state to provided state.
    /// </summary>
    /// <param name="name">The name of the state to change to.</param>
    /// <param name="parameters">Optional enter methods parameters.</param>
    public void ChangeState(string name, Dictionary<string, object>? parameters = null)
    {
        StateMachine?.Change(name, parameters);
    }

    /// <summary>
    /// Called when the dice's sprite should be updated.
    /// </summary>
    /// <param name="texturePath">The path to the new texture for the game entity.</param>
    /// <exception cref="Exception">Thrown when the texture path is invalid or wasn't able to retrieve the entity texture.</exception>
    public void UpdateTexture(string texturePath)
    {
        // Entity Texture.
        try
        {
            EntityTexture = TextureAtlas.FromFile(_contentManager, texturePath); 
        } 
        catch
        {
            throw new Exception($"Failed to load new texture in GameEntity. Texture: {texturePath}.");
        }
    }

    /// <summary>
    /// Updates the animation of the game entity.
    /// </summary>
    /// <param name="animationName">The name of the animation to switch to.</param>
    /// <exception cref="Exception">Thrown when couldn't retrieve new animation.</exception>
    public void UpdateAnimation(string animationName, int totalCycles = int.MaxValue)
    {
        // Animation
        try
        {
            Vector2 offset = CurrentAnimation.Origin;
            Color color = CurrentAnimation.Color;
            CurrentAnimation = EntityTexture.CreateAnimatedSprite(animationName, totalCycles);
            CurrentAnimationName = animationName;
            CurrentAnimation.Origin = offset;
            CurrentAnimation.Color = color;
        } 
        catch
        {
            throw new Exception($"Failed to load new animation in GameEntity. Animation name: {animationName}");
        }
    }
    #endregion Methods
}
