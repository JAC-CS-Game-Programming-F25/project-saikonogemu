using System;
using System.Collections.Generic;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

namespace Game.Scripts.Entities.Dice;

/// <summary>
/// Creates a new TargetDice entity instance.
/// </summary>
/// <param name="content">The content manager used by the scene to load in content.</param>
/// <param name="diceDefinition">All the dice specific parameters.</param>
public class TargetDice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : NPCDice(content, diceDefinition)
{
    #region Constants
    public const float VISION_RANGE = 200;
    public const float VISION_BUFFER = 20;
    #endregion Constants

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        UpdateVisionScope();
    }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
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
                height = Hitbox.Collider.Height - VISION_BUFFER;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(offset, offset);
                Vision = new RectangleFloat(position, width, height);
            break;

            case DiceDirections.Left:
                width = VISION_RANGE;
                height = Hitbox.Collider.Height - VISION_BUFFER;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(-offset, offset);
                Vision = new RectangleFloat(new Vector2(position.X - (width - Hitbox.Collider.Width), position.Y), width, height);
            break;

            case DiceDirections.Up:
                width = Hitbox.Collider.Width - VISION_BUFFER;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(offset, -offset);
                Vision = new RectangleFloat(new Vector2(position.X, position.Y  - (height - Hitbox.Collider.Height)), width, height);
            break;

            case DiceDirections.Down:
                width = Hitbox.Collider.Width - VISION_BUFFER;
                height = VISION_RANGE;
                offset = VISION_BUFFER / 2;
                position = Hitbox.Collider.Position + new Vector2(offset, offset);
                Vision = new RectangleFloat(position, width, height);
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
        if (!IsValidDirectionSwapTime())
            return;

        if (Vision.Intersects(player.Hitbox.Collider))
        {
            NewDiceDirection = OppositeDiceDirection(DiceDirection);
            ResetTimer();
        }
    }
    #endregion Methods
}