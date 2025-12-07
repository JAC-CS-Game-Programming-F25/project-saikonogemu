using System;
using System.Collections.Generic;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoreLibrary.Utils;

#nullable enable

public static class Utils
{
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

        Texture2D _debugRed = new Texture2D(Core.GraphicsDevice, 1, 1);
            _debugRed.SetData([Color.Red]);

        foreach (var r in PhysicsManager.Instance.TileColliders)
        {
            // Fill.
            Rectangle drawRect = new Rectangle(
                (int)Math.Floor(r.Left),
                (int)Math.Floor(r.Top),
                (int)Math.Ceiling(r.Width),
                (int)Math.Ceiling(r.Height)
            );

            // Outline.
            Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width, OUTLINE_SIZE), Color.Red);
            Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Bottom - OUTLINE_SIZE, drawRect.Width, OUTLINE_SIZE), Color.Red);
            Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Left, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Red);
            Core.SpriteBatch.Draw(_debugRed, new Rectangle(drawRect.Right - OUTLINE_SIZE, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Red);
        }

        Texture2D _debugOrange = new Texture2D(Core.GraphicsDevice, 1, 1);
        _debugOrange.SetData([Color.Orange]);

        foreach (var r in PhysicsManager.Instance.RigidBodies)
        {
            Rectangle drawRect = new Rectangle(
                (int)Math.Floor(r.Collider.Left),
                (int)Math.Floor(r.Collider.Top),
                (int)Math.Ceiling(r.Collider.Width),
                (int)Math.Ceiling(r.Collider.Height)
            );

            // Outline.
            Core.SpriteBatch.Draw(_debugOrange, new Rectangle(drawRect.Left, drawRect.Top, drawRect.Width, OUTLINE_SIZE), Color.Orange);
            Core.SpriteBatch.Draw(_debugOrange, new Rectangle(drawRect.Left, drawRect.Bottom - OUTLINE_SIZE, drawRect.Width, OUTLINE_SIZE), Color.Orange);
            Core.SpriteBatch.Draw(_debugOrange, new Rectangle(drawRect.Left, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Orange);
            Core.SpriteBatch.Draw(_debugOrange, new Rectangle(drawRect.Right - OUTLINE_SIZE, drawRect.Top, OUTLINE_SIZE, drawRect.Height), Color.Orange);
        }
    }
    #endregion Helper Methods
}