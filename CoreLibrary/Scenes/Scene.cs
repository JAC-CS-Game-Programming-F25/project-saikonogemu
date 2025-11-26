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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CoreLibrary.Scenes;

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
    public virtual void Update(GameTime gameTime) { }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public virtual void Draw(GameTime gameTime) { }

    #endregion Update and Draw

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
}
