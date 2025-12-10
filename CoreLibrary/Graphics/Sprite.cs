/***************************************************************
 * File: Sprite.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Sprite class represents a drawable 2D image or texture region.
 *  It encapsulates rendering properties such as position, rotation,
 *  scaling, color masking, and layer depth. This class can be used
 *  directly for static images or extended for more complex behaviors
 *  like animations.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pleasing;

namespace CoreLibrary.Graphics;

/// <summary>
/// Represents a 2D image that can be drawn to the screen with
/// customizable rendering properties such as rotation, scaling,
/// color masking, and layer depth.
/// </summary>
public class Sprite
{
    #region Properties

    /// <summary>
    /// Gets or sets the source texture region represented by this sprite.
    /// </summary>
    public TextureRegion Region { get; set; }

    /// <summary>
    /// Gets or sets the color mask to apply when rendering this sprite.
    /// </summary>
    /// <remarks>Default value is <see cref="Color.White"/>.</remarks>
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the amount of rotation, in radians, to apply when rendering this sprite.
    /// </summary>
    /// <remarks>Default value is <c>0.0f</c>.</remarks>
    public float Rotation { get; set; } = 0.0f;

    /// <summary>
    /// Gets or sets the scale factor to apply to the X and Y axes when rendering this sprite.
    /// </summary>
    /// <remarks>Default value is <see cref="Vector2.One"/>.</remarks>
    public Vector2 Scale { get; set; } = Vector2.One;

    /// <summary>
    /// Gets or sets the XY-coordinate origin point, relative to the top-left corner, of this sprite.
    /// </summary>
    /// <remarks>Default value is <see cref="Vector2.Zero"/>.</remarks>
    public Vector2 Origin { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the sprite effects to apply when rendering this sprite.
    /// </summary>
    /// <remarks>Default value is <see cref="SpriteEffects.None"/>.</remarks>
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;

    /// <summary>
    /// Gets or sets the layer depth to apply when rendering this sprite.
    /// </summary>
    /// <remarks>Default value is <c>0.0f</c>.</remarks>
    public float LayerDepth { get; set; } = 0.0f;

    /// <summary>
    /// Gets the width, in pixels, of this sprite after applying scale.
    /// </summary>
    public float Width => Region.Width * Scale.X;

    /// <summary>
    /// Gets the height, in pixels, of this sprite after applying scale.
    /// </summary>
    public float Height => Region.Height * Scale.Y;

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class.
    /// </summary>
    public Sprite() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sprite"/> class
    /// using the specified texture region.
    /// </summary>
    /// <param name="region">The texture region to associate with this sprite.</param>
    public Sprite(TextureRegion region)
    {
        Region = region;
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Tweens the opacity of the sprite.
    /// </summary>
    /// <param name="duration">How long the tweening should take.</param>
    /// <param name="targetOpacity">The target finishing opacity.</param>
    /// <param name="easing">The easing we want to use, Linear is the default.</param>
    /// <returns>The tweening timeline so we can stop/repeat the tween.</returns>
    public TweenTimeline TweenOpacity(float duration, float targetOpacity, EasingFunction easing = null)
    {
        // This will make sure the opacity is a valid float.
        targetOpacity = MathHelper.Clamp(targetOpacity, 0f, 1f);

        // The initial color of the rendering.
        Color startColor = Color;
        int targetAlpha = MathHelper.Clamp((int)Math.Round(targetOpacity * 255f), 0, 255);
        Color targetColor = new Color(startColor.R, startColor.G, startColor.B, targetAlpha);

        // Creates a tweening timeline (makes it so we can repeat/stop it).
        TweenTimeline timeline = Tweening.NewTimeline();
        timeline.AdaptiveDuration = true;

        TweenableProperty<Color> colorProp = timeline.AddColor(this, "Color");

        // We'll use Linear easing by default.
        EasingFunction ease = easing ?? Easing.Linear;

        // Add the start and end frames (like checkpoints, super helpful for linked tweens).
        colorProp.AddFrame(0f, startColor, ease);
        colorProp.AddFrame(duration, targetColor, ease);

        return timeline;
    }

    /// <summary>
    /// Sets the origin point of this sprite to its center.
    /// </summary>
    public void CenterOrigin()
    {
        Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
    }

    /// <summary>
    /// Draws this sprite using the specified <see cref="SpriteBatch"/>.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch instance used for batching draw calls.</param>
    /// <param name="position">The XY-coordinate position to render this sprite at.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        Region.Draw(spriteBatch, position, Color, Rotation, Origin, Scale, Effects, LayerDepth);
    }

    #endregion Public Methods
}
