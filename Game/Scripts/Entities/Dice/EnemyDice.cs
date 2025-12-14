using System.Collections.Generic;
using CoreLibrary.Physics;
using Game.Scripts.Entities.Dice.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

namespace Game.Scripts.Entities.Dice;

/// <summary>
/// Creates a new EnemyDice entity instance.
/// </summary>
/// <param name="content">The content manager used by the scene to load in content.</param>
/// <param name="diceDefinition">All the dice specific parameters.</param>
public class EnemyDice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : NPCDice(content, diceDefinition)
{
    #region Constants
    public const float VISION_RANGE = 200;
    public const float VISION_BUFFER = 20;
    private const float SPEED_BUFF = 25;
    private const float GHOST_SPAWN_INTERVAL = 0.5f;
    private const float GHOST_INITIAL_OPACITY = 0.8f;
    private const float GHOST_FADE_DURATION = 1000f;
    #endregion Constants

    #region Properties
    private readonly List<EnemyDice> _ghosts = new();
    private float _ghostSpawnTimer = 0f;
    private bool _isGhost;
    #endregion Properties

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_isGhost)
            return;

        // Updates all the ghost.
        for (int i = _ghosts.Count - 1; i >= 0; i--)
        {
            EnemyDice ghost = _ghosts[i];
            ghost.Update(gameTime);

            // Removes all old ghosts.
            if (ghost.DiceOpacity <= 0.01f)
                _ghosts.RemoveAt(i);
        }

        UpdateVisionScope();
    }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        if (_isGhost)
            return;

        // Renders all the ghost.
        foreach(EnemyDice ghost in _ghosts)
            ghost.Draw(gameTime);
    }

    #endregion Update and Draw

    #region Methods
    private void UpdateVisionScope()
    {
        float width;
        float height;
        float offset;
        Vector2 position;

        // Update vision (struct).
        switch (DiceDirection)
        {
            case DiceDirections.Right:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(offset, 0);
                Vision = new RectangleFloat(new Vector2(position.X, Hitbox.Collider.Position.Y - (height - Hitbox.Collider.Height) / 2), width, height);
            break;

            case DiceDirections.Left:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(-offset, 0);
                Vision = new RectangleFloat(new Vector2(position.X - (width - Hitbox.Collider.Width), Hitbox.Collider.Position.Y - (height - Hitbox.Collider.Height) / 2), width, height);
            break;

            case DiceDirections.Up:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(0, -offset);
                Vision = new RectangleFloat(new Vector2(Hitbox.Collider.Position.X - (width - Hitbox.Collider.Width) / 2, position.Y  - (height - Hitbox.Collider.Height)), width, height);
            break;

            case DiceDirections.Down:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(0, offset);
                Vision = new RectangleFloat(new Vector2(Hitbox.Collider.Position.X - (width - Hitbox.Collider.Width) / 2, position.Y), width, height);
            break;

            case DiceDirections.DownLeft:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(-offset, offset);
                Vision = new RectangleFloat(new Vector2(position.X - (width - Hitbox.Collider.Width), position.Y), width, height);
            break;

            case DiceDirections.DownRight:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(offset, offset);
                Vision = new RectangleFloat(position, width, height);
            break;

            case DiceDirections.UpLeft:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(-offset, -offset);
                Vision = new RectangleFloat(new Vector2(position.X - (width - Hitbox.Collider.Width), position.Y  - (height - Hitbox.Collider.Height)), width, height);
            break;

            case DiceDirections.UpRight:
                width = VISION_RANGE;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(offset, -offset);
                Vision = new RectangleFloat(new Vector2(position.X, position.Y  - (height - Hitbox.Collider.Height)), width, height);
            break;
        }
    }

    /// <summary>
    /// Handles player in vision by running.
    /// </summary>
    public override void HandlePlayerVisionCollision(PlayerDice player, GameTime gameTime)
    {
        if (Vision.Intersects(player.Hitbox.Collider))
        {
            if (IsTrackingPlayer)
                Speed = (SPEED_BUFF + SPEED) * Scale.X;

            IsTrackingPlayer = true;
            HandleGhostGeneration(gameTime);
        }
        else
        {
            if (IsTrackingPlayer && player.DiceDirection != DiceDirections.Idle)
                // One last hope ;-;.
                NewDiceDirection = player.DiceDirection;

            Speed = SPEED * Scale.X;
            IsTrackingPlayer = false;
        }
    }

    /// <summary>
    /// Generates ghosts based on a timer.
    /// </summary>
    /// <param name="gameTime">The gameTime of the Game.</param>
    private void HandleGhostGeneration(GameTime gameTime)
    {
        _ghostSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_ghostSpawnTimer >= GHOST_SPAWN_INTERVAL)
        {
            _ghostSpawnTimer = 0f;
            SpawnGhost();
        }
    }

    /// <summary>
    /// Spawns a ghost copy of the player.
    /// </summary>
    private void SpawnGhost()
    {
        // Makes a ghost clone.
        EnemyDice ghost = CreateGhost();

        // Starts ghost slightly transparent.
        ghost.DiceOpacity = GHOST_INITIAL_OPACITY;

        // Tweens if out.
        ghost.TweenOpacity(GHOST_FADE_DURATION, 0);

        // Adds it to the list to render.
        _ghosts.Add(ghost);
    }

    /// <summary>
    /// Clones the EnemyDice instance as a ghost.
    /// </summary>
    /// <returns>Returns the cloned item.</returns>
    public EnemyDice CreateGhost()
    {
        // Clones the player.
        EnemyDice ghost = (EnemyDice)MemberwiseClone();

        // Copies only what we need.
        ghost.CurrentAnimation = CurrentAnimation.Clone();
        ghost.DiceOpacity = DiceOpacity;

        // Snapshot the position
        ghost.Hitbox = Hitbox.Clone();
        ghost.Hitbox.Velocity = Vector2.Zero;
        ghost.Hitbox.Dynamic = false;

        // Ghost should not update states.
        ghost.StateMachine = null;

        ghost._ghosts.Clear();
        ghost._isGhost = true;

        return ghost;
    }

    #endregion Methods
}