/***************************************************************
 * File: TextureRegion.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The TextureRegion class represents a rectangular subsection
 *  of a texture. It defines a source area that can be drawn using
 *  a SpriteBatch, allowing efficient reuse of a single texture
 *  for multiple sprites or frames of an animation.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoreLibrary.Graphics;

/// <summary>
/// Represents a rectangular region within a texture.
/// </summary>
public class TextureRegion
{
    #region Properties

    /// <summary>
    /// Gets or sets the texture this region is part of.
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Gets or sets the source rectangle that defines the area of the texture
    /// used by this region.
    /// </summary>
    public Rectangle SourceRectangle { get; set; }

    /// <summary>
    /// Gets the width, in pixels, of this texture region.
    /// </summary>
    public int Width => SourceRectangle.Width;

    /// <summary>
    /// Gets the height, in pixels, of this texture region.
    /// </summary>
    public int Height => SourceRectangle.Height;

    /// <summary>
    /// Gets the top normalized texture coordinate of this region.
    /// </summary>
    public float TopTextureCoordinate => SourceRectangle.Top / (float)Texture.Height;

    /// <summary>
    /// Gets the bottom normalized texture coordinate of this region.
    /// </summary>
    public float BottomTextureCoordinate => SourceRectangle.Bottom / (float)Texture.Height;

    /// <summary>
    ///  Gets the left normalized texture coordinate of this region.
    /// </summary>
    public float LeftTextureCoordinate => SourceRectangle.Left / (float)Texture.Width;

    /// <summary>
    /// Gets the right normalized texture coordinate of this region.
    /// </summary>
    public float RightTextureCoordinate => SourceRectangle.Right / (float)Texture.Width;

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="TextureRegion"/> class.
    /// </summary>
    public TextureRegion() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureRegion"/> class using the specified texture and region bounds.
    /// </summary>
    /// <param name="texture">The source texture.</param>
    /// <param name="x">The X-coordinate of the upper-left corner of the region.</param>
    /// <param name="y">The Y-coordinate of the upper-left corner of the region.</param>
    /// <param name="width">The width, in pixels, of the region.</param>
    /// <param name="height">The height, in pixels, of the region.</param>
    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
    {
        Texture = texture;
        SourceRectangle = new Rectangle(x, y, width, height);
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Draws this texture region to the specified sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    /// <param name="position">The screen position to draw the region at.</param>
    /// <param name="color">The color mask to apply when rendering.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        Draw(spriteBatch, position, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
    }

    /// <summary>
    /// Draws this texture region with rotation and scaling applied.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    /// <param name="position">The screen position to draw the region at.</param>
    /// <param name="color">The color mask to apply when rendering.</param>
    /// <param name="rotation">The rotation, in radians.</param>
    /// <param name="origin">The origin point for rotation and scaling.</param>
    /// <param name="scale">The uniform scale factor to apply.</param>
    /// <param name="effects">Optional sprite flipping effects.</param>
    /// <param name="layerDepth">The draw layer depth.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        Draw(spriteBatch, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
    }

    /// <summary>
    /// Draws this texture region with rotation, scaling, and layer depth applied.
    /// </summary>
    /// <param name="spriteBatch">The SpriteBatch used for drawing.</param>
    /// <param name="position">The screen position to draw the region at.</param>
    /// <param name="color">The color mask to apply when rendering.</param>
    /// <param name="rotation">The rotation, in radians.</param>
    /// <param name="origin">The origin point for rotation and scaling.</param>
    /// <param name="scale">The scale factor to apply to both axes.</param>
    /// <param name="effects">Optional sprite flipping effects.</param>
    /// <param name="layerDepth">The draw layer depth.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(
            Texture,
            position,
            SourceRectangle,
            color,
            rotation,
            origin,
            scale,
            effects,
            layerDepth
        );
    }

    #endregion Public Methods
}
