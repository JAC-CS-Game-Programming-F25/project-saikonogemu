/***************************************************************
 * File: Core.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Core class serves as the central game engine manager.
 *  It initializes and maintains core systems such as graphics,
 *  content, input, and physics, and handles the update and draw
 *  cycles for the active Scene. Core also manages transitions
 *  between scenes and provides global access to key systems.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CoreLibrary.Input;
using CoreLibrary.Audio;
using CoreLibrary.Scenes;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework.Audio;

namespace CoreLibrary;

/// <summary>
/// Represents the main entry point and management system for the game engine.
/// Responsible for initializing and maintaining global subsystems such as
/// graphics, input, physics, and scene management.
/// </summary>
public class Core : Game
{
    #region Fields

    internal static Core s_instance;

    private static Scene s_activeScene;
    private static Scene s_nextScene;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => s_instance;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>
    /// Gets a reference to the input management system.
    /// </summary>
    public static InputManager Input { get; private set; }

    /// <summary>
    /// Gets a reference to the audio control system.
    /// </summary>
    public static AudioController Audio { get; private set; }

    /// <summary>
    /// Gets a reference to the physics manager system.
    /// </summary>
    public static PhysicsManager Physics { get; private set; }

    /// <summary>
    /// Gets or sets a value that indicates if the game should exit when the Escape key is pressed.
    /// </summary>
    public static bool ExitOnEscape { get; set; }

    /// <summary>
    /// Whether to show debug utils.
    /// </summary>
    public static bool DebugMode {get; set;}

    /// <summary>
    /// Gets the setting manager.
    /// </summary>
    public static SettingsManager SettingsManager {get; private set;}

    /// <summary>
    /// The sound effect the UI makes when you move the selection.
    /// </summary>
    public static SoundEffect UISoundEffect;

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Core"/> class.
    /// Configures the game window, graphics, and basic runtime properties.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    public Core(string title)
    {
        // Ensure that multiple cores are not created.
        if (s_instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        // Store reference to engine for global member access.
        s_instance = this;

        SettingsManager = new SettingsManager();
        SettingsManager.Load();

        // Create a new graphics device manager.
        Graphics = new GraphicsDeviceManager(this);

        // Set the graphics defaults.
        Graphics.PreferredBackBufferWidth = SettingsManager.Graphics.Width;
        Graphics.PreferredBackBufferHeight = SettingsManager.Graphics.Height;
        Graphics.IsFullScreen = SettingsManager.Graphics.Fullscreen;

        // Apply the graphic presentation changes.
        Graphics.ApplyChanges();

        // Set the window title.
        Window.Title = title;

        // Set the core's content manager to a reference of the base Game's content manager.
        Content = base.Content;

        // Set the root directory for content.
        Content.RootDirectory = "Content";

        // Mouse is visible by default.
        IsMouseVisible = true;

        // Exit on escape is true by default.
        ExitOnEscape = true;
    }

    #endregion Constructors

    #region Game Lifecycle

    /// <summary>
    /// Initializes all core systems, including graphics, input, and physics managers.
    /// </summary>
    protected override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // Set the core's graphics device to a reference of the base Game's graphics device.
        GraphicsDevice = base.GraphicsDevice;

        // Create the sprite batch instance.
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        if (Graphics.IsFullScreen)
        {
            Graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        }

        Graphics.SynchronizeWithVerticalRetrace = true;
        IsFixedTimeStep = false;

        Graphics.ApplyChanges();

        // Create a new input manager.
        Input = new InputManager();

        // Create a new audio controller.
        Audio = new AudioController();

        Audio.SongVolume = SettingsManager.Audio.MusicVolume;
        Audio.SoundEffectVolume = SettingsManager.Audio.SfxVolume;

        // Initialize the physics system.
        Physics = PhysicsManager.Instance;
    }

    /// <summary>
    /// Unloads global content and performs cleanup when the game is shutting down.
    /// </summary>
    protected override void UnloadContent()
    {
        // Saves all the settings.
        SettingsManager.Audio.MusicVolume = Audio.SongVolume;
        SettingsManager.Audio.SfxVolume = Audio.SoundEffectVolume;
        SettingsManager.Graphics.Fullscreen = Graphics.IsFullScreen;
        SettingsManager.Graphics.Width = Graphics.PreferredBackBufferWidth;
        SettingsManager.Graphics.Height = Graphics.PreferredBackBufferHeight;
        SettingsManager.Save();

        // Dispose of the audio controller.
        Audio.Dispose();

        base.UnloadContent();
    }

    /// <summary>
    /// Updates the engine state, including input, physics, and the active scene.
    /// Handles scene transitions and exits the game when requested.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        // Update the input manager.
        Input.Update(gameTime);

        // Update the audio controller.
        Audio.Update();

        // Check for escape key exit.
        if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // If there is a next scene waiting to switch to, transition to it.
        if (s_nextScene != null)
        {
            TransitionScene();
        }

        // If there is an active scene, update it.
        if (s_activeScene != null)
        {
            Physics.Update(gameTime);
            s_activeScene.Update(gameTime);
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the currently active scene to the screen.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        // If there is an active scene, draw it.
        if (s_activeScene != null)
        {
            s_activeScene.Draw(gameTime);
        }

        base.Draw(gameTime);
    }

    #endregion Game Lifecycle

    #region Scene Management

    /// <summary>
    /// Queues a new scene to be activated at the next update cycle.
    /// </summary>
    /// <param name="next">The next scene to switch to.</param>
    public static void ChangeScene(Scene next)
    {
        // Only set the next scene value if it is not the same instance as the currently active scene.
        if (s_activeScene != next)
        {
            s_nextScene = next;
        }
    }

    /// <summary>
    /// Handles transitioning between scenes, including disposing of the previous scene,
    /// resetting physics, and initializing the new scene.
    /// </summary>
    private static void TransitionScene()
    {
        // If there is an active scene, dispose of it.
        if (s_activeScene != null)
        {
            s_activeScene.Dispose();
        }

        // Reset the physics system.
        Physics.Reset();

        // Force garbage collection to clear memory from the old scene.
        GC.Collect();

        // Change the currently active scene to the new scene.
        s_activeScene = s_nextScene;

        // Null out the next scene value so it does not trigger a change over and over.
        s_nextScene = null;

        // If the active scene is valid, initialize it (also calls LoadContent internally).
        if (s_activeScene != null)
        {
            s_activeScene.Initialize();
        }
    }

    #endregion Scene Management
}
