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
    // FIXME: Adjust the fields to properties as needed. Might want to make protected too.

    // TODO: The camera might be moved to Scene.
    // The camera for the scene (what the player sees)
    private Camera _camera = new Camera();

    // The bounds of the screen (the physical bounds of the player's screen (e.g. 1920x1080)).
    // These are always exact numbers.
    private Rectangle _screenBounds;

    // The tilemap of the scene.
    private Tilemap _tilemap;

    // FIXME: This shouldn't be AnimatedSprite.
    private List<AnimatedSprite> _dice;
    // FIXME: this is a temp to test animations/textures
    private AnimatedSprite player;
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

        // Loads all the dice textures initially.
        // The player dice which texture will change will be done in it's States.
        LoadInitDice();

        // FIXME: Move to Level
        _tilemap = Tilemap.FromFile(Content, "Images/Levels/XML/level4.xml");
        _tilemap.Scale = new Vector2(3.0f, 3.0f);

        // TODO: Initialize player, initialize enemies, initialize targets. Use dice factory. Pass in dice to PlayState.
        // Adds the states to the state machine.
        StateMachine.Add("PlayState", new PlayState());

        // TODO: Start game scene song here.
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
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
        // FIXME: This will be changed to Dice instead of AnimatedSprite.
        foreach (AnimatedSprite die in _dice)
        {
            // FIXME: This should use the stored position from the dice.
            die.Draw(Core.SpriteBatch, Vector2.Zero);
        }

        // Ends the drawing.
        Core.SpriteBatch.End();
    }
    #endregion Scene Lifecycle

    #region Methods
    public void LoadInitDice()
    {
        // Creates a TextureAtlas from the XML file. (this stores all the sprites for this scene)
        // This is where all the animations are gotten from the XML and can be given to sprites etc.

        // FIXME: This will be moved to the DiceStates.
        // Player Dice
        TextureAtlas playerTextureAtlas = TextureAtlas.FromFile(Content, "Images/Atlas/player_dice_atlas.xml");
        player = playerTextureAtlas.CreateAnimatedSprite("player_dice_dot6_vertical_animation");
        player.Scale = new Vector2(3.0f, 3.0f);

        // Target Dice
        //TextureAtlas targetTextureAtlas = TextureAtlas.FromFile(Content, "Images/Atlas/target_dice_atlas.xml");

        // Enemy Dice
        //TextureAtlas enemyTextureAtlas = TextureAtlas.FromFile(Content, "Images/Atlas/enemy_dice_atlas.xml");
    }
    #endregion Methods
}