using System;
using System.Collections.Generic;
using System.Linq;
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
    public const float SPEED = 75f;
    public const float DIRECTION_SWAP_COOLDOWN = 0.5f;
    private float _directionSwapTimer = 0f;

    #region Properties
    public RectangleFloat Vision {get; set;} = new RectangleFloat();
    public DiceDirections NewDiceDirection {get; set;}
    public bool IsTrackingPlayer {get; set;}
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

        Vision = new RectangleFloat(Hitbox.Collider.Position, Hitbox.Collider.Width, Hitbox.Collider.Height);

        _directionSwapTimer = DIRECTION_SWAP_COOLDOWN;
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

        _directionSwapTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

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

    #region Methods
    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void ResetTimer()
    {
        _directionSwapTimer = DIRECTION_SWAP_COOLDOWN;
    }

    /// <summary>
    /// Checks whether now is a valid time to swap directions.
    /// </summary>
    /// <returns>Whether it's a valid time.</returns>
    public bool IsValidDirectionSwapTime()
    {
        return _directionSwapTimer <= 0;
    }

    /// <summary>
    /// Handles NPC to NPC collision.
    /// </summary>
    public void HandleNPCCollision()
    {
        if(!IsValidDirectionSwapTime())
            return;

        NewDiceDirection = OppositeDiceDirection(DiceDirection);
        Knockback();
        ResetTimer();
    }

    /// <summary>
    /// Function to be overridden by children.
    /// </summary>
    public virtual void HandlePlayerVisionCollision(PlayerDice player, GameTime gameTime) { }
    #endregion Methods
}