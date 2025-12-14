using System;
using System.Collections.Generic;
using System.IO;
using CoreLibrary;
using CoreLibrary.Audio;
using CoreLibrary.Graphics;
using CoreLibrary.Physics;
using CoreLibrary.Scenes;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice;
using Game.Scripts.Levels;
using Game.Scripts.Scenes.EndingScenes;
using Game.Scripts.Scenes.GameSceneItems.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Pleasing;

namespace Game.Scripts.Scenes.GameSceneItems;

public class GameScene : Scene
{
    #region Greeting
    private const float HEADER_OFFSET_MULTIPLIER = 0.45f;
    private const float MESSAGE_OFFSET_MULTIPLIER = 1.25f;
    private const float HEADER_SCALE = 6f;
    private const float SHADOW_OFFSET = 5f;

    // The position to draw the message text at.
    private Vector2 _messageTextPosition;

    // The origin to set for the message text.
    private Vector2 _messageTextOrigin;

    // The font used to render the header text.
    private SpriteFont _headerFont;

    // The position to draw the header text at.
    private Vector2 _headerTextPosition;

    // The origin to set for the header text.
    private Vector2 _headerTextOrigin;

    private const float AFTER_FADE_DURATION = 2.0f;

    private float? _disappearingTimer;

    private bool _isMessage;
    private bool _showText = true;
    #endregion Greeting

    #region Constants
    private const float GAME_SCALE = 3.0f;
    private const float BACKGROUND_CHANGE_DURATION = 115000f; 
    #endregion Constants

    #region  Fields
    // The bounds of the screen (the physical bounds of the player's screen (e.g. 1920x1080)).
    // These are always exact numbers.
    private Rectangle _screenBounds;

    // The tilemap of the scene.
    private Tilemap _tilemap;

    // List of dice present in the level.
    private List<Dice> _dice = [];    

    // List of tiles around the player that future dice cannot spawn on.
    private List<int> _bannedSpawnTiles = [];

    // This is ONLY used once, if it's used after initialization there's a problem.
    private int _initialPlayerHealth;

    // The previous position of the player. Needed for camera tracking.
    private Vector2 _oldPlayerPosition;

    // The speed of the fade to grayscale effect.
    public const float FADE_SPEED = 0.02f;
    #endregion Fields

    #region Properties
    /// <summary>
    /// The current level this game scene is for.
    /// </summary>
    public Level CurrentLevel {get; private set;}

    public bool IsPlayerDead {get; set;}

    #endregion Properties

    #region Scene Lifecycle

    /// <summary>
    /// The very start of the core processor of the game.
    /// </summary>
    /// <param name="levelType">The level that this game scene is running.</param>
    public GameScene(LevelType levelType, int initialPlayerHealth = 6) : base()
    {
        // Loads the info for the current level.
        CurrentLevel = Level.FromFile(Content, $"Data/Levels/level{(int)levelType}_data.xml");
        _initialPlayerHealth = initialPlayerHealth + CurrentLevel.extraLife;

        if (_initialPlayerHealth > 6)
            _initialPlayerHealth = 6;
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

        #region Greeting Setup
        // Dimensions.
        int screenWidth  = Core.GraphicsDevice.Viewport.Width;
        int screenHeight = Core.GraphicsDevice.Viewport.Height;

        // Header.
        Vector2 headerSize = _headerFont.MeasureString(CurrentLevel.header);
        _headerTextPosition = new Vector2(screenWidth / 2f, screenHeight / 2f * HEADER_OFFSET_MULTIPLIER);
        _headerTextOrigin = headerSize / 2f;

        if (CurrentLevel.message != null)
        {
            _isMessage = true;

            // Message.
            Vector2 messageSize = _headerFont.MeasureString(CurrentLevel.message);
            _messageTextPosition = new Vector2(screenWidth / 2f, (_headerTextPosition.Y + headerSize.Y) * MESSAGE_OFFSET_MULTIPLIER);
            _messageTextOrigin = messageSize / 2f;
        }
        #endregion Greeting Setup

        // Sets the screenBounds based on the player's screen.
        _screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

        // Adds collidable tiles to the Physics.
        PhysicsManager.Instance.TileColliders = _tilemap.GetNearbyColliders(
          new RectangleFloat(0, 0, _tilemap.Columns * _tilemap.TileWidth, _tilemap.Rows * _tilemap.TileHeight)
        );

        #region Initialize Dice
        // Initializes the player.
        InitializePlayer();

        // Initializes the Targets.
        InitializeTargets();

        // Initializes the Enemies.
        InitializeEnemies();
        #endregion Initialize Dice

        // Set camera after player and screenBounds are made.
        Vector2 playerPosition = _dice[0].Hitbox.Collider.Centre;
        Vector2 screenCenter = new(_screenBounds.Width / 2f, _screenBounds.Height / 2f);
        Camera.Instance.Translation = -(screenCenter - playerPosition);

        // Get the initial player position.
        _oldPlayerPosition = _dice[0].Hitbox.Collider.Centre;

        // Adds the states to the state machine.
        StateMachine.Add("PlayState", new PlayState(), new Dictionary<string, object> { ["dice"] = _dice, ["gameScene"] = this});
        StateMachine.Add("PauseState", new PauseState());
    }

    /// <summary>
    /// LoadContent comes right before the Initialize method.
    /// It's where all the game assets are set up.
    /// </summary>
    public override void LoadContent()
    {
        base.LoadContent();

        // Sets up the map.
        _tilemap = Tilemap.FromFile(Content, CurrentLevel.tilemapPath);
        _tilemap.Scale = new Vector2(GAME_SCALE, GAME_SCALE);

        // Sets up the color of the level.
        BackgroundColor = CurrentLevel.color;
        TweenBackground(BACKGROUND_CHANGE_DURATION, Color.Black);

        _headerFont = Content.Load<SpriteFont>("Fonts/peaberrybase_font");

        // Play song.
        Song song = Content.Load<Song>($"Audio/Music/{CurrentLevel.songName}");
        Core.Audio.PlaySong(song);
    }

    public override void Update(GameTime gameTime)
    {
        CheckForGameOver();
        CheckForLevelProgression();

        if (_disappearingTimer != null)
            _disappearingTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        base.Update(gameTime);

        // Moves the camera.
        HandleCameraMovement();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        // PointClamp makes it so that the nearest pixel is selected without anti-aliasing, etc.
        // If you don't know what a samplerState is, see https://docs.monogame.net/articles/getting_to_know/whatis/graphics/WhatIs_Sampler.html
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.Instance.TransformationMatrix);

        // Draws the tilemap floor.
        _tilemap.DrawLayer(Core.SpriteBatch, 0);

        // We draw all the dice here since we want to be able to see them in both paused and play states.
        // Depending on the state it will be rendered differently, that is handled in the state itself.
        foreach (Dice die in _dice)
        {
            die.CurrentAnimation.Scale = die.Scale;
            die.Draw(gameTime);
        }

        // Draws the Border layer afterwards
        _tilemap.DrawLayer(Core.SpriteBatch, 1);

        if (Core.DebugMode)
            Utils.DrawColliders();

        // Ends the drawing.
        Core.SpriteBatch.End();

        DrawStateMachine(gameTime);

        AfterDraw(gameTime);

        if (_showText)
        {
            if (!IsFading && _disappearingTimer is null)
                _disappearingTimer = AFTER_FADE_DURATION;

            if (_disappearingTimer < 0.0f && _disappearingTimer != null)
                _showText = false;

            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // The color to use for the drop shadow text.
            Color dropShadowColor = CurrentLevel.color * 0.5f;

            // Draw the Header text slightly offset from it is original position and
            // with a transparent color to give it a drop shadow
            Core.SpriteBatch.DrawString(_headerFont, CurrentLevel.header, _headerTextPosition + new Vector2(SHADOW_OFFSET, SHADOW_OFFSET), dropShadowColor, 0.0f, _headerTextOrigin, HEADER_SCALE, SpriteEffects.None, 1.0f);

            // Draw the Header text on top of that at its original position
            Core.SpriteBatch.DrawString(_headerFont, CurrentLevel.header, _headerTextPosition, Color.White, 0.0f, _headerTextOrigin, HEADER_SCALE, SpriteEffects.None, 1.0f);

            if (_isMessage)
            {
                // Draw the Message text slightly offset from it is original position and
                // with a transparent color to give it a drop shadow
                Core.SpriteBatch.DrawString(_headerFont, CurrentLevel.message, _messageTextPosition + new Vector2(SHADOW_OFFSET, SHADOW_OFFSET), dropShadowColor, 0.0f, _messageTextOrigin, HEADER_SCALE - 2, SpriteEffects.None, 1.0f);

                // Draw the Message text on top of that at its original position
                Core.SpriteBatch.DrawString(_headerFont, CurrentLevel.message, _messageTextPosition, Color.White, 0.0f, _messageTextOrigin, HEADER_SCALE - 2, SpriteEffects.None, 1.0f);                
            }

            // Always end the sprite batch when finished.
            Core.SpriteBatch.End();
        }
    }
    #endregion Scene Lifecycle

    #region Methods
    /// <summary>
    /// Check ran to see if the game is over.
    /// </summary>
    private void CheckForGameOver()
    {
        if (IsPlayerDead)
        {
            if (!IsExiting)
                ExitScene();

            if (IsFinishedExiting)
                ChangeToGameOver();
        }
    }

    /// <summary>
    /// Checks and implements level progression if needed.
    /// </summary>
    private void CheckForLevelProgression()
    {
        int totalNumberOfTargets = 0;

        foreach(Dice dice in _dice)
            if (dice is TargetDice)
                totalNumberOfTargets ++;

        if (totalNumberOfTargets == 0)
        {
            foreach (Dice dice in _dice)
                dice.Hitbox.Velocity = Vector2.Zero;

            if (!IsExiting)
                ExitScene();

            if (IsFinishedExiting)
                ChangeLevel();
        }
    }

    /// <summary>
    /// Handles your death.
    /// </summary>
    private void ChangeToGameOver()
    {
        Core.ChangeScene(new GameOverScene());
    }

    /// <summary>
    /// Handles the change to a new level if possible.
    /// </summary>
    private void ChangeLevel()
    {
        if (CurrentLevel.type == LevelType.Level9)
        {
            Core.ChangeScene(new VictoryScene(_dice[0].Health));
        }
        else
        {
            Core.ChangeScene(new GameScene((LevelType)((int)CurrentLevel.type + 1), _dice[0].Health));
        }
    }

    /// <summary>
    /// Handles the movement of the camera.
    /// </summary>
    private void HandleCameraMovement()
    {
        Camera.Instance.Translation = Camera.Instance.Translation += _dice[0].Hitbox.Collider.Centre - _oldPlayerPosition;
        _oldPlayerPosition = _dice[0].Hitbox.Collider.Centre;
    }

    /// <summary>
    /// Responsible to initializing the player and for generating banned tiles.
    /// </summary>
    private void InitializePlayer()
    {
        ValueTuple<int, int> playerLocationIndex = CalculateSpawnIndex();
        _bannedSpawnTiles = GenerateBannedTiles(playerLocationIndex);

        _dice.Add(CreateDice(DiceTypes.Player, _initialPlayerHealth, playerLocationIndex, PlayerDice.SPEED));
    }

    /// <summary>
    /// Responsible to initializing the targets.
    /// </summary>
    private void InitializeTargets()
    {
        foreach(int targetHealth in CurrentLevel.targets)
        {
            ValueTuple<int, int> targetLocationIndex = CalculateSpawnIndex();

            // So they don't spawn on top of each other xD.
            int bannedIndex = targetLocationIndex.Item2 * _tilemap.Columns + targetLocationIndex.Item1;
            _bannedSpawnTiles.Add(bannedIndex);

            _dice.Add(CreateDice(DiceTypes.Target, targetHealth, targetLocationIndex, NPCDice.SPEED));
        }
    }

    /// <summary>
    /// Responsible to initializing the targets.
    /// </summary>
    private void InitializeEnemies()
    {
        foreach(int enemyHealth in CurrentLevel.enemies)
        {
            ValueTuple<int, int> enemyLocationIndex = CalculateSpawnIndex();

            // So they don't spawn on top of each other xD.
            int bannedIndex = enemyLocationIndex.Item2 * _tilemap.Columns + enemyLocationIndex.Item1;
            _bannedSpawnTiles.Add(bannedIndex);

            _dice.Add(CreateDice(DiceTypes.Enemy, enemyHealth, enemyLocationIndex, NPCDice.SPEED));
        }
    }

    /// <summary>
    /// Creates a dice at a location by using the <see cref="DiceFactory"/>.
    /// </summary>
    /// <param name="diceType">The type of the dice <see cref="DiceTypes"/>.</param>
    /// <param name="location">The location to spawn the dice (ValueTuple).</param>
    /// <returns>The new dice created.</returns>
    private Dice CreateDice(DiceTypes diceType, int diceHealth, ValueTuple<float, float> location, float speed = 100f)
    {
        string diceTextureName = diceType switch {
            DiceTypes.Player => "player_dice",
            DiceTypes.Target => "target_dice",
            DiceTypes.Enemy => "enemy_dice",
            _ => "invalid_dice_texture",
        };

        return DiceFactory.CreateDice(
            diceType,
            Content, 
            new Dictionary<string, object> 
            {
                ["texture"] = $"Images/Atlas/{diceTextureName}_atlas.xml",
                ["animationName"] = $"{diceTextureName}_dot{diceHealth}_idle_animation",
                ["position"] = new Vector2(location.Item1 * _tilemap.TileWidth, location.Item2 * _tilemap.TileHeight),
                ["positionOffset"] = new Vector2(Dice.NORMAL_OFFSET, Dice.NORMAL_OFFSET),
                ["sizeOffset"] = new Vector2(-10, -10),
                ["scale"] = new Vector2(GAME_SCALE, GAME_SCALE),
                ["speed"] = speed,
                ["entityTotalHealth"] = diceHealth,
                ["hasDash"] = CurrentLevel.hasDash,
                ["hasPhase"] = CurrentLevel.hasPhase,
            }
        );
    }

    /// <summary>
    /// Generates a list of tiles.
    /// </summary>
    /// <param name="location">The location to ban tiles around.</param>
    private List<int> GenerateBannedTiles(ValueTuple<int, int> location)
    {
        // We calculate the column and row.
        int column = location.Item1;
        int row = location.Item2;

        // Now we generate a perimeter of banned tiles. It's an array of tuples
        ValueTuple<int, int>[] bannedTileZone =
        [
            (-2, -2), (-1, -2), (0, -2), (+1, -2), (+2, -2),
            (-2, -1), (-1, -1), (0, -1), (+1, -1), (+2, -1),
            (-2,  0), (-1,  0), (0,  0), (+1,  0), (+2,  0),
            (-2, -1), (-1, +1), (0, +1), (+1, +1), (+2, +1),
            (-2, +2), (-1, +2), (0, +2), (+1, +2), (+2, +2),
        ];

        List<int> bannedTiles = [];

        // For each coordinate we ban that index for future dice.
        foreach (ValueTuple<int, int> tuple in bannedTileZone)
        {
            int bannedColumn = column + tuple.Item1;
            int bannedRow = row + tuple.Item2;

            int bannedIndex = bannedRow * _tilemap.Columns + bannedColumn;
            bannedTiles.Add(bannedIndex);
        }

        return bannedTiles;
    }

    /// <summary>
    /// Generates a valid spawnable index for a dice.
    /// </summary>
    /// <returns>Returns the spawn index for the dice.</returns>
    private ValueTuple<int, int> CalculateSpawnIndex()
    {
        // Start a random generator.
        Random random = new Random();

        int randomIndex;

        do
        {
            // Gets a random index.
            randomIndex = random.Next(0, _tilemap.Count);

            // Loops until the index is a spawnable location for the dice.
        } while(_tilemap.IsCollidable(randomIndex) || _tilemap.GetTileId(randomIndex, 0) == 0 || _bannedSpawnTiles.Contains(randomIndex));
        
        int column = randomIndex % _tilemap.Columns;
        int row = randomIndex / _tilemap.Columns;

        // Once the index is valid.
        return (column, row);
    }
    #endregion Methods
}