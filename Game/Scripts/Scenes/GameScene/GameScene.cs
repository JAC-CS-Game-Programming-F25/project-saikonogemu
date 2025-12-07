using System;
using System.Collections.Generic;
using System.IO;
using CoreLibrary;
using CoreLibrary.Graphics;
using CoreLibrary.Physics;
using CoreLibrary.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DieTheRollingDiceGame;

public class GameScene : Scene
{
    #region  Fields
    private Texture2D _debugRed;

    // TODO: The camera might be moved to Scene.
    // The camera for the scene (what the player sees)
    private Camera _camera = new Camera();

    // The bounds of the screen (the physical bounds of the player's screen (e.g. 1920x1080)).
    // These are always exact numbers.
    private Rectangle _screenBounds;

    // The tilemap of the scene.
    private Tilemap _tilemap;

    // List of dice present in the level.
    private List<Dice> _dice = new List<Dice>();
    #endregion Fields

    #region Scene Lifecycle

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

        // Sets the screenBounds based on the player's screen.
        _screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
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
                ["positionOffset"] = new Vector2(-5, -5),
                ["sizeOffset"] = new Vector2(-10, -10),
                ["scale"] = new Vector2(3.0f, 3.0f),
                ["entityTotalHealth"] = 6
            }
        ));

        // Adds the states to the state machine.
        StateMachine.Add("PlayState", new PlayState(), new Dictionary<string, object> { ["dice"] = _dice});

        // TODO: REMOVE THIS
        _debugRed = new Texture2D(Core.GraphicsDevice, 1, 1);
        _debugRed.SetData(new[] { Color.White });

    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        
        // FIXME: Make it so it works with the camera smh.
        // Adds collidable tiles to the Physics.
        PhysicsManager.Instance.TileColliders = _tilemap.GetNearbyColliders(
            new RectangleFloat(0,0, _tilemap.Columns * 32 * 3, _tilemap.Rows * 32 * 3)
        );
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        // Clears the canvas back to a black screen (can be any color I like black since it looks like the void :D).
        Core.GraphicsDevice.Clear(Color.Black);

        // PointClamp makes it so that the nearest pixel is selected without anti-aliasing, etc.
        // If you don't know what a samplerState is, see https://docs.monogame.net/articles/getting_to_know/whatis/graphics/WhatIs_Sampler.html
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.TransformationMatrix);

        // Draws the tilemap (we want to see it even if paused).
        _tilemap.Draw(Core.SpriteBatch);

        // We draw all the dice here since we want to be able to see them in both paused and play states.
        // Depending on the state it will be rendered differently, that is handled in the state itself.
        foreach (Dice die in _dice)
        {
            die.Draw(gameTime);
        }

        // TODO: DELETE ME
        if (PhysicsManager.Instance.TileColliders != null)
        {
            foreach (var r in PhysicsManager.Instance.TileColliders)
            {
                // r is RectangleFloat: convert to Rectangle for SpriteBatch draw
                Rectangle drawRect = new Rectangle(
                    (int)Math.Floor(r.Left),
                    (int)Math.Floor(r.Top),
                    (int)Math.Ceiling(r.Width),
                    (int)Math.Ceiling(r.Height)
                );

                // outline: 1px thick borders
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width, 1), Color.Red);
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Bottom - 1, drawRect.Width, 1), Color.Red);
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Top, 1, drawRect.Height), Color.Red);
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Right - 1, drawRect.Top, 1, drawRect.Height), Color.Red);
            }

            foreach (var r in PhysicsManager.Instance.RigidBodies)
            {
                // r is RectangleFloat: convert to Rectangle for SpriteBatch draw
                Rectangle drawRect = new Rectangle(
                    (int)Math.Floor(r.Collider.Left),
                    (int)Math.Floor(r.Collider.Top),
                    (int)Math.Ceiling(r.Collider.Width),
                    (int)Math.Ceiling(r.Collider.Height)
                );

                // fill with translucent red
                Core.SpriteBatch.Draw(_debugRed, drawRect, new Color(255, 0, 0, 80));

                // outline: 1px thick borders
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width, 1), Color.Red);
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Bottom - 1, drawRect.Width, 1), Color.Red);
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Top, 1, drawRect.Height), Color.Red);
                Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Right - 1, drawRect.Top, 1, drawRect.Height), Color.Red);
            }
        }

        // Ends the drawing.
        Core.SpriteBatch.End();
    }
    #endregion Scene Lifecycle
}