using System;
using System.Collections.Generic;
using System.IO;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Physics;
using CoreLibrary.Scenes;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice;
using Game.Scripts.Levels;
using Game.Scripts.Scenes.GameScene.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pleasing;

namespace Game.Scripts.Scenes.GameScene;

public class GameScene : Scene
{
    #region Constants
    // FIXME: Change to match song length??? MAYBE?
    private const float BACKGROUND_CHANGE_DURATION = 20000f;
    #endregion Constants

    #region  Fields
    // The camera for the scene (what the player sees)
    private Camera _camera = new();

    // The bounds of the screen (the physical bounds of the player's screen (e.g. 1920x1080)).
    // These are always exact numbers.
    private Rectangle _screenBounds;

    // The bounds of the room.
    private Rectangle _roomBounds;

    // The tilemap of the scene.
    private Tilemap _tilemap;

    // List of dice present in the level.
    private List<Dice> _dice = new List<Dice>();

    private Vector2? _oldPlayerPosition = null;
    #endregion Fields

    #region Properties
    public Level CurrentLevel {get; private set;}
    #endregion Properties

    #region Scene Lifecycle

    /// <summary>
    /// The very start of the core processor of the game.
    /// </summary>
    /// <param name="levelType">The level that this game scene is running.</param>
    public GameScene(LevelType levelType) : base()
    {
        // Loads the info for the current level.
        CurrentLevel = Level.FromFile(Content, $"Data/Levels/level{(int)levelType}_data.xml");
    }

    /// <summary>
    /// The very first method ran when the scene is started.
    /// It's like an entering method. 
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        // When we are in the game scene (any level for that matter), we don't want be quit the game when ESC is pressed.
        // Instead, the ESC key will be used to pause the game.
        Core.ExitOnEscape = false;

        // Sets the mouse as not visible.
        Core.Instance.IsMouseVisible = false;

        // Sets the screenBounds based on the player's screen.
        _screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

        _camera.Translation =  new Vector2(-(_screenBounds.Width - _roomBounds.Width) / 2, -(_screenBounds.Height - _roomBounds.Height) / 2);
    }

    /// <summary>
    /// LoadContent comes right after the Initialize method.
    /// It's where all the game assets are set up.
    /// </summary>
    public override void LoadContent()
    {
        base.LoadContent();

        // FIXME: Move to Level
        _tilemap = Tilemap.FromFile(Content, "Images/Levels/XML/level1.xml");
        _tilemap.Scale = new Vector2(3.0f, 3.0f);

        // FIXME: Move to level
        _roomBounds = new Rectangle(
            0,
            0,
            _tilemap.Columns * (int)_tilemap.TileWidth,
            _tilemap.Rows * (int)_tilemap.TileHeight
        );

        // FIXME: Change to use Level.Color
        BackgroundColor = Color.Green;
        TweenBackground(BACKGROUND_CHANGE_DURATION, Color.Black);

        // TODO: Initialize player, initialize enemies, initialize targets. Use dice factory. Pass in dice to PlayState.


        // TODO: Start game scene song here.


        // FIXME: delete this when it comes time
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;

        _dice.Add(
            new PlayerDice(Content, 
            new Dictionary<string, object> 
            {
                ["texture"] = "Images/Atlas/player_dice_atlas.xml",
                ["animationName"] = "player_dice_dot6_idle_animation",
                ["position"] = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight),
                ["positionOffset"] = new Vector2(Dice.NORMAL_OFFSET, Dice.NORMAL_OFFSET),
                ["sizeOffset"] = new Vector2(-10, -10),
                ["scale"] = new Vector2(3.0f, 3.0f),
                ["entityTotalHealth"] = 6
            }
        ));

        // Adds the states to the state machine.
        StateMachine.Add("PlayState", new PlayState(), new Dictionary<string, object> { ["dice"] = _dice});
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_oldPlayerPosition != null)
        {
            Vector2 newPosition = _camera.Translation += _dice[0].Hitbox.Collider.Centre - (Vector2)_oldPlayerPosition;

            _camera.Translation = newPosition;
        }

        _oldPlayerPosition = _dice[0].Hitbox.Collider.Centre;
        
        // Adds collidable tiles to the Physics.
        PhysicsManager.Instance.TileColliders = _tilemap.GetNearbyColliders(
            _camera.GetBounds(_screenBounds.Width, _screenBounds.Height)
        );
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        // PointClamp makes it so that the nearest pixel is selected without anti-aliasing, etc.
        // If you don't know what a samplerState is, see https://docs.monogame.net/articles/getting_to_know/whatis/graphics/WhatIs_Sampler.html
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.TransformationMatrix);

        // Draws the tilemap floor.
        _tilemap.DrawLayer(Core.SpriteBatch, 0);

        // We draw all the dice here since we want to be able to see them in both paused and play states.
        // Depending on the state it will be rendered differently, that is handled in the state itself.
        foreach (Dice die in _dice)
        {
            die.Draw(gameTime);
        }

        // Draws the Border layer afterwards
        _tilemap.DrawLayer(Core.SpriteBatch, 1);

        // TODO: ADD debug mode
        if (PhysicsManager.Instance.TileColliders != null)
        {
            Utils.DrawColliders();
        }

        // Ends the drawing.
        Core.SpriteBatch.End();
    }
    #endregion Scene Lifecycle
}