/***************************************************************
 * File: Scene.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Scene class serves as the abstract base for all game scenes.
 *  It provides structure for scene lifecycle management including
 *  initialization, content loading, updating, drawing, and disposal.
 *  Each derived Scene manages its own content through a unique
 *  ContentManager instance to allow clean unloading on scene exit.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Pleasing;

namespace CoreLibrary.Scenes;

#nullable enable

/// <summary>
/// Represents the base class for all game scenes.
/// Provides lifecycle methods for initialization, content management,
/// updating, drawing, and disposal.
/// </summary>
public abstract class Scene : IDisposable
{
    #region Properties

    /// <summary>
    /// Gets the ContentManager used for loading scene-specific assets.
    /// </summary>
    /// <remarks>
    /// Assets loaded through this ContentManager will be automatically unloaded when this scene ends.
    /// </remarks>
    protected ContentManager Content { get; }

    /// <summary>
    /// Gets a value that indicates if the scene has been disposed of.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets the statemachine of the entity.
    /// </summary>
    public StateMachine StateMachine { get; set; } = new StateMachine();

    /// <summary>
    /// The background color of the scene.
    /// </summary>
    public Color BackgroundColor { get; set; } = Color.Black;
    #endregion Properties

    #region Constructors

    /// <summary>
    /// Creates a new scene instance.
    /// </summary>
    public Scene()
    {
        // Create a content manager for the scene.
        Content = new ContentManager(Core.Content.ServiceProvider)
        {
            // Set the root directory for content to match the game's root directory.
            RootDirectory = Core.Content.RootDirectory
        };
    }

    // Finalizer, called when object is cleaned up by the garbage collector.
    ~Scene() => Dispose(false);

    #endregion Constructors

    #region Lifecycle Methods

    /// <summary>
    /// Initializes the scene.
    /// </summary>
    /// <remarks>
    /// When overriding this in a derived class, ensure that base.Initialize()
    /// is still called as this is when LoadContent is invoked.
    /// </remarks>
    public virtual void Initialize()
    {
        LoadContent();
    }

    /// <summary>
    /// Override to provide logic to load content for the scene.
    /// </summary>
    public virtual void LoadContent() { }

    /// <summary>
    /// Unloads scene-specific content.
    /// </summary>
    public virtual void UnloadContent()
    {
        Content.Unload();
    }

    #endregion Lifecycle Methods

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public virtual void Update(GameTime gameTime)
    {
        // Update all tweens.
        Tweening.Update(gameTime);

        // Updates the StateMachine.
        StateMachine?.Update(gameTime);
    }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public virtual void Draw(GameTime gameTime)
    {
        // Clears the canvas back to the backgroundColor;
        Core.GraphicsDevice.Clear(BackgroundColor);

        // Draws the StateMachine.
        StateMachine?.Draw(gameTime);
    }

    #endregion Update and Draw

    #region Methods
    /// <summary>
    /// Changes the current state to provided state.
    /// </summary>
    /// <param name="name">The name of the state to change to.</param>
    /// <param name="parameters">Optional enter methods parameters.</param>
    public void ChangeState(string name, Dictionary<string, object>? parameters = null)
    {
        StateMachine?.Change(name, parameters);
    }
    #endregion Methods

    #region Disposal

    /// <summary>
    /// Disposes of this scene.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of this scene.
    /// </summary>
    /// <param name="disposing">
    /// Indicates whether managed resources should be disposed. This value is only true when called from the main
    /// Dispose method. When called from the finalizer, this will be false.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;

        if (disposing)
        {
            UnloadContent();
            Content.Dispose();
        }

        IsDisposed = true;
    }

    #endregion Disposal

    #region Public Methods
    /// <summary>
    /// Tweens the opacity of the background.
    /// </summary>
    /// <param name="duration">How long the tweening should take.</param>
    /// <param name="targetOpacity">The target finishing opacity.</param>
    /// <param name="easing">The easing we want to use, Linear is the default.</param>
    /// <returns>The tweening timeline so we can stop/repeat the tween.</returns>
    public TweenTimeline TweenBackground(float duration, Color targetColor, EasingFunction? easing = null)
    {
        // The initial color of the rendering.
        Color startColor = BackgroundColor;

        // Creates a tweening timeline (makes it so we can repeat/stop it).
        TweenTimeline timeline = Tweening.NewTimeline();
        timeline.AdaptiveDuration = true;

        TweenableProperty<Color> colorProp = timeline.AddColor(this, nameof(BackgroundColor));

        // We'll use Linear easing by default.
        EasingFunction ease = easing ?? Easing.Linear;

        // Add the start and end frames (like checkpoints, super helpful for linked tweens).
        colorProp.AddFrame(0f, startColor, ease);
        colorProp.AddFrame(duration, targetColor, ease);

        return timeline;
    }
    #endregion Public Methods
}
