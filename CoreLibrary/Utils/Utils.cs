using System;
using System.Collections.Generic;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoreLibrary.Utils;

#nullable enable

public static class Utils
{
    private static Texture2D? _pixel;

    private static Texture2D Pixel
    {
        get
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(Core.GraphicsDevice, 1, 1);
                _pixel.SetData([Color.White]);
            }
            return _pixel;
        }
    }

    #region Helper Methods   
    /// <summary>
    /// Gets the potentially null value from a potentially null dictionary.
    /// </summary>
    /// <typeparam name="T">The type we are trying to convert to.</typeparam>
    /// <param name="dictionary">The potentially null dictionary.</param>
    /// <param name="key">The key to the potentially null or invalid value.</param>
    /// <param name="fallback">The fallback default.</param>
    /// <returns>The fallback value if it fails; the value itself otherwise.</returns>
    public static T GetValue<T>(Dictionary<string, object>? dictionary, string key, T fallback)
    {
        if (dictionary != null && dictionary.TryGetValue(key, out var value) && value is T castValue)
            return castValue;
        return fallback;
    }

    /// <summary>
    /// Draws outlines around colliders.
    /// TileColliders are Red, Rigidbodies are Orange.
    /// </summary>
    public static void DrawColliders()
    {
        const int OUTLINE_SIZE = 5;

        foreach (RectangleFloat r in PhysicsManager.Instance.TileColliders)
        {
            // Fill.
            Rectangle drawRect = new Rectangle((int)Math.Floor(r.Left), (int)Math.Floor(r.Top), (int)Math.Ceiling(r.Width), (int)Math.Ceiling(r.Height));

            // Outline.
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width, OUTLINE_SIZE), Color.Red);
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Bottom - OUTLINE_SIZE, drawRect.Width, OUTLINE_SIZE), Color.Red);
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Red);
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Right - OUTLINE_SIZE, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Red);
        }

        foreach (Rigidbody r in PhysicsManager.Instance.RigidBodies)
        {
            Rectangle drawRect = new Rectangle((int)Math.Floor(r.Collider.Left), (int)Math.Floor(r.Collider.Top), (int)Math.Ceiling(r.Collider.Width), (int)Math.Ceiling(r.Collider.Height));

            // Outline.
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width, OUTLINE_SIZE), Color.Orange);
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Bottom - OUTLINE_SIZE, drawRect.Width, OUTLINE_SIZE), Color.Orange);
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Orange);
            Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Right - OUTLINE_SIZE, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Orange);
        }
    }

    /// <summary>
    /// Draws the outlines of the provided rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="color">The color of the draw.</param>
    /// <param name="outlineSize">The size of the outline, default is 5.</param>
    public static void DrawRectangle(RectangleFloat rect, Color color, int outlineSize = 5)
    {
        Rectangle drawRect = new Rectangle((int)Math.Floor(rect.Left), (int)Math.Floor(rect.Top), (int)Math.Ceiling(rect.Width), (int)Math.Ceiling(rect.Height));

        // Top.
        Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width, outlineSize), color);

        // Bottom.
        Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Bottom - outlineSize, drawRect.Width, outlineSize), color);

        // Left.
        Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Left, drawRect.Top, outlineSize, drawRect.Height), color);

        // Right.
        Core.SpriteBatch.Draw(Pixel, new Rectangle(drawRect.Right - outlineSize, drawRect.Top, outlineSize, drawRect.Height), color);
    }

    /// <summary>
    /// Gets a Color from the provided hex value.
    /// </summary>
    /// <param name="hex">The hex of the color.</param>
    /// <returns>Returns the Color received from the hex value.</returns>
    public static Color FromHex(string hex)
    {
        hex = hex.Replace("#", "");
        int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color(r, g, b);
    }
    #endregion Helper Methods
}