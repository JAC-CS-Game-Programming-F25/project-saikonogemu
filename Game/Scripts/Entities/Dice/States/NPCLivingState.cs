using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CoreLibrary;
using CoreLibrary.Input;
using CoreLibrary.Physics;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#nullable enable

namespace Game.Scripts.Entities.Dice.States;

/// <summary>
/// Represents the state of the npc dice in living form.
/// </summary>
public class NPCLivingState : DiceLivingState
{
    #region Constants
    // 5 seconds.
    private const double DIRECTION_CHANGE_TIME = 3.0f;

    // 1 second.
    private const float WALL_COOLDOWN_TIME = 0.5f;
    #endregion Constants

    #region Backing Fields
    private float _directionChangeTimer = 0f;
    private float _wallCooldown = 0f;
    #endregion Backing Fields

    #region Lifecycle Methods
    /// <summary>
    /// Called when entering this State.
    /// </summary>
    /// <param name="parameters">Optional parameters needed from other states.</param>
    public override void Enter(Dictionary<string, object>? parameters = null)
    {
        base.Enter(parameters);
        UpdateDirection();
    }

    /// <summary>
    /// Called when exiting this State.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime">The GameTime of the game.</param>
    public override void Update(GameTime gameTime)
    {
        if (Dice!.IsFrozen)
            return;

        base.Update(gameTime);

        HandleMovement(gameTime);
    }

    /// <summary>
    /// Called every GameTime while this state is active.
    /// </summary>
    /// <param name="gameTime">The GameTime of the game.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
    #endregion Lifecycle Methods

    #region Input Handling

    /// <summary>
    /// Handles NPC movement.
    /// </summary>
    private void HandleMovement(GameTime gameTime)
    {
        // detections!
        _wallCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        // When the NPC's vision sees a wall.
        HandleVisionSeesWall();
        
        // Timer that decides when to switch directions
        HandleDirectionTimer(gameTime);

        // Adjust velocity for new direction (if any).
        UpdateVelocity(((NPCDice)Dice!).NewDiceDirection);
    }

    /// <summary>
    /// Updates velocity accordingly to dice direction.
    /// </summary>
    private void UpdateVelocity(DiceDirections newDirection)
    {
        Vector2 selectDelta = Vector2.Zero;

        if (newDirection is DiceDirections.Up or DiceDirections.UpLeft or DiceDirections.UpRight) selectDelta.Y -= 1;

        if (newDirection is DiceDirections.Down or DiceDirections.DownLeft or DiceDirections.DownRight) selectDelta.Y += 1;

        if (newDirection is DiceDirections.Left or DiceDirections.UpLeft or DiceDirections.DownLeft) selectDelta.X -= 1;

        if (newDirection is DiceDirections.Right or DiceDirections.UpRight or DiceDirections.DownRight) selectDelta.X += 1;

        // We normalize if it's possible (this makes diagonals the same speed as horizontals/verticals).
        if (selectDelta != Vector2.Zero)
        {
            selectDelta = Vector2.Normalize(selectDelta) * Dice!.Speed;
        }

        // Modifies the player's velocity.
        Dice!.Hitbox.Velocity = selectDelta;
    }

    /// <summary>
    /// Handles when the dice sees a wall.
    /// </summary>
    private void HandleVisionSeesWall()
    {
        if (_wallCooldown > 0f)
            return;

        // Checks every tile relevant.
        foreach (RectangleFloat tile in PhysicsManager.Instance.TileColliders)
        {
            // If the entity’s vision intersects with a tile collider.
            if (((NPCDice)Dice!).Vision.Intersects(tile))
            {
                // We don't want it constantly reevaluating.
                _wallCooldown = WALL_COOLDOWN_TIME;

                HandlePreferredCollisionDirection();
            }
        }
    }

    /// <summary>
    /// Will calculate the intersection side and calculate the proper direction to ideally turn to.
    /// </summary>
    public void HandlePreferredCollisionDirection()
    {
        // Why do this? Well as an example if your vision picks up a wall up and you are moving right, you should update but not go up or right.
        switch (Dice!.DiceDirection)
        {
            case DiceDirections.Right:
                UpdatePreferredDirection(DiceDirections.Left, [DiceDirections.Right, DiceDirections.UpRight, DiceDirections.DownRight]);
            break;

            case DiceDirections.Left:
                UpdatePreferredDirection(DiceDirections.Right, [DiceDirections.Left, DiceDirections.UpLeft, DiceDirections.DownLeft]);
            break;

            case DiceDirections.Up:
                UpdatePreferredDirection(DiceDirections.Down, [DiceDirections.Up, DiceDirections.UpLeft, DiceDirections.UpRight]);
            break;

            case DiceDirections.Down:
                UpdatePreferredDirection(DiceDirections.Up, [DiceDirections.Down, DiceDirections.DownLeft, DiceDirections.DownRight]);
            break;

            case DiceDirections.DownLeft:
                UpdatePreferredDirection(DiceDirections.UpRight, [DiceDirections.DownLeft, DiceDirections.Down, DiceDirections.Left]);
            break;

            case DiceDirections.DownRight:
                UpdatePreferredDirection(DiceDirections.UpLeft, [DiceDirections.DownRight, DiceDirections.Down, DiceDirections.Right]);
            break;

            case DiceDirections.UpLeft:
                UpdatePreferredDirection(DiceDirections.DownRight, [DiceDirections.UpLeft, DiceDirections.Up, DiceDirections.Left]);
            break;

            case DiceDirections.UpRight:
                UpdatePreferredDirection(DiceDirections.DownLeft, [DiceDirections.UpRight, DiceDirections.Right, DiceDirections.Up]);
            break;
        }
    }

    /// <summary>
    /// Gets a new direction thats not the current one.
    /// </summary>
    private void UpdatePreferredDirection(DiceDirections preferred, List<DiceDirections>? diceDirectionToIgnore = null)
    {
        // Means we've changed it somewhere else and don't want to override.
        if (((NPCDice)Dice!).NewDiceDirection != Dice!.DiceDirection)
            return;

        // Handle the empty list.
        diceDirectionToIgnore ??= [];

        // More LINQ, this gets the dice directions that are valid for the switch. The weird [..] came from when I did .ToArray and VSCode asked if I wanted a simplified equation.
        // Note, the preferred direction is added twice :D.
        DiceDirections[] candidates = [.. Enum.GetValues<DiceDirections>().Where(d => d != Dice!.DiceDirection && d != DiceDirections.Idle).Where(d => !diceDirectionToIgnore.Contains(d)).Append(preferred)];

        if (candidates.Length == 0)
            return;

        // Accidentally found Random.Shared.Next and it seems to be better than making a new one each time.
        ((NPCDice)Dice!).NewDiceDirection = candidates[Random.Shared.Next(candidates.Length)];
        
        _directionChangeTimer = 0;
    }

    /// <summary>
    /// Gets a new direction thats not the current one.
    /// </summary>
    private void UpdateDirection(List<DiceDirections>? diceDirectionToIgnore = null)
    {
        // Means we've changed it somewhere else and don't want to override.
        if (((NPCDice)Dice!).NewDiceDirection != Dice!.DiceDirection)
            return;

        // Handle the empty list.
        diceDirectionToIgnore ??= [];

        // More LINQ, this gets the dice directions that are valid for the switch. The weird [..] came from when I did .ToArray and VSCode asked if I wanted a simplified equation.
        DiceDirections[] candidates = [.. Enum.GetValues<DiceDirections>().Where(d => d != Dice!.DiceDirection && d != DiceDirections.Idle).Where(d => !diceDirectionToIgnore.Contains(d))];

        if (candidates.Length == 0)
            return;

        // Accidentally found Random.Shared.Next and it seems to be better than making a new one each time.
        ((NPCDice)Dice!).NewDiceDirection = candidates[Random.Shared.Next(candidates.Length)];
        
        _directionChangeTimer = 0;
    }



    /// <summary>
    /// Handles the timer for when to change directions.
    /// </summary>
    /// <param name="gameTime">The gameTime of the Game.</param>
    private void HandleDirectionTimer(GameTime gameTime)
    {
        _directionChangeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_directionChangeTimer >= DIRECTION_CHANGE_TIME && !((NPCDice)Dice!).IsTrackingPlayer)
        {
            _directionChangeTimer = 0;
            UpdateDirection();
        }
    }
    #endregion Input Handling
}