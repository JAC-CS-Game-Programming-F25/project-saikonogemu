using System.Collections.Generic;
using CoreLibrary;
using CoreLibrary.Physics;
using CoreLibrary.Utils;
using Game.Scripts.Entities.Dice.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

#nullable enable

namespace Game.Scripts.Entities.Dice;

public class NPCDice: Dice
{
    public const float VISION_RANGE = 200;
    public const float SPEED = 75f;

    #region Properties
    public RectangleFloat Vision {get; set;} = new RectangleFloat();
    #endregion Properties

    #region Constructors
    /// <summary>
    /// Creates a new TargetDice entity instance.
    /// </summary>
    /// <param name="content">The content manager used by the scene to load in content.</param>
    /// <param name="diceDefinition">All the dice specific parameters.</param>
    public NPCDice(ContentManager content, Dictionary<string, object>? diceDefinition = null) : base(content, diceDefinition)
    {
        // Adds the player neutral state.
        AddState("NPCLivingState", new NPCLivingState(), new Dictionary<string, object> { ["dice"] = this });

        // Adds the player phase state.
        AddState("DiceDyingState", new DiceDyingState());

        Vision = new RectangleFloat(Hitbox.Collider.Position - new Vector2(VISION_RANGE / 2, VISION_RANGE / 2), Hitbox.Collider.Width + VISION_RANGE, Hitbox.Collider.Height + VISION_RANGE);
    }
    #endregion Constructors

    #region Update and Draw

    /// <summary>
    /// Updates this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Update(GameTime gameTime)
    {
        if (IsFrozen)
            return;

        // Update vision (struct).
        Vision = new RectangleFloat(Hitbox.Collider.Position - new Vector2(VISION_RANGE / 2, VISION_RANGE / 2), Vision.Width, Vision.Height);

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws this scene.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        if (Core.DebugMode)
            Utils.DrawRectangle(Vision, Color.AliceBlue);
    }

    #endregion Update and Draw
}