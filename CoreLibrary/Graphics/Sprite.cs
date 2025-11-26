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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
