/***************************************************************
 * File: RectangleFloat.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The RectangleFloat struct represents a rectangle using floating-point
 *  precision instead of integers. It provides properties and methods
 *  for position, size, intersection detection, translation, and equality
 *  comparison, making it suitable for precise physics and graphics
 *  calculations in 2D environments.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using Microsoft.Xna.Framework;

namespace CoreLibrary.Physics;

/// <summary>
/// Rectangle struct that uses floats (instead of integers).
/// 
/// Coordinate system starts from the top-left,
/// so (Left, Top) is the origin point.
/// </summary>
public struct RectangleFloat : IEquatable<RectangleFloat>
{
    #region Fields

    /// <summary>
    /// Used to calculate Left and Right properties.
    /// </summary>
    private float _x;

    /// <summary>
    /// Used to calculate Top and Bottom properties.
    /// </summary>
    private float _y;

    #endregion Fields

    #region Properties

    /// <summary>
    /// The width of the rectangle.
    /// </summary>
    public float Width;

    /// <summary>
    /// The height of the rectangle.
    /// </summary>
    public float Height;

    /// <summary>
    /// The left side of the rectangle.
    /// </summary>
    public float Left
    {
        readonly get => _x;
        set => _x = value;
    }

    /// <summary>
    /// The right side of the rectangle.
    /// </summary>
    public float Right
    {
        readonly get => _x + Width;
        set => _x = value - Width;
    }

    /// <summary>
    /// The top side of the rectangle.
    /// </summary>
    public float Top
    {
        readonly get => _y;
        set => _y = value;
    }

    /// <summary>
    /// The bottom side of the rectangle.
    /// </summary>
    public float Bottom
    {
        readonly get => _y + Height;
        set => _y = value - Height;
    }

    /// <summary>
    /// The position of the rectangle (Left, Top).
    /// </summary>
    public Vector2 Position
    {
        readonly get => new Vector2(_x, _y);
        set
        {
            _x = value.X;
            _y = value.Y;
        }
    }

    /// <summary>
    /// The size of the rectangle (Width, Height).
    /// </summary>
    public readonly Vector2 Size => new Vector2(Width, Height);

    /// <summary>
    /// The center of the rectangle.
    /// </summary>
    public readonly Vector2 Centre => new Vector2(Left + Width * 0.5f, Top + Height * 0.5f);

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Constructor for RectangleFloat.
    /// </summary>
    /// <param name="x">The starting x position of the rectangle.</param>
    /// <param name="y">The starting y position of the rectangle.</param>
    /// <param name="width">The starting width of the rectangle.</param>
    /// <param name="height">The starting height of the rectangle.</param>
    /// <exception cref="ArgumentException">Thrown if width or height are less than or equal to 0.</exception>
    public RectangleFloat(float x, float y, float width, float height)
    {
        if (width < 0 || height < 0)
            throw new ArgumentException("Width or Height of RectangleFloat cannot be less than zero.");

        _x = x;
        _y = y;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Constructor with position vector.
    /// </summary>
    public RectangleFloat(Vector2 pos, float width, float height) 
        : this(pos.X, pos.Y, width, height) 
    { }

    #endregion Constructors

    #region Static Methods

    /// <summary>
    /// Translates a provided rectangle by a displacement.
    /// Does not modify the original rectangle.
    /// </summary>
    public static RectangleFloat Translate(RectangleFloat rect, Vector2 displacement) =>
        new RectangleFloat(rect.Left + displacement.X, rect.Top + displacement.Y, rect.Width, rect.Height);

    /// <summary>
    /// Checks if two rectangles intersect.
    /// </summary>
    public static bool Intersects(RectangleFloat rectA, RectangleFloat rectB) => rectA.Intersects(rectB);

    #endregion Static Methods

    #region Public Methods

    /// <summary>
    /// Translates this rectangle by a displacement.
    /// </summary>
    public void Translate(Vector2 displacement)
    {
        _x += displacement.X;
        _y += displacement.Y;
    }

    /// <summary>
    /// Checks if this rectangle intersects another.
    /// </summary>
    public readonly bool Intersects(RectangleFloat rect) =>
        rect.Left < Right && Left < rect.Right && rect.Top < Bottom && Top < rect.Bottom;

    /// <summary>
    /// String representation.
    /// </summary>
    public override readonly string ToString() =>
        $"{{X:{Left} Y:{Top} Width:{Width} Height:{Height}}}";

    /// <summary>
    /// Returns a value that indicates whether this RectangleFloat and the specified object are equal.
    /// </summary>
    /// <param name="obj">The object to compare with this RectangleFloat.</param>
    /// <returns>Whether this RectangleFloat and the specified object are equal.</returns>
    public override readonly bool Equals(object obj) =>
        obj is RectangleFloat other && Equals(other);

    /// <summary>
    /// Returns a value that indicates whether this RectangleFloat and the specified RectangleFloat are equal.
    /// </summary>
    /// <param name="rect">The RectangleFloat to compare with this RectangleFloat.</param>
    /// <returns>Whether this RectangleFloat and the specified RectangleFloat are equal.</returns>
    public readonly bool Equals(RectangleFloat rect) =>
        Left == rect.Left && Top == rect.Top && Width == rect.Width && Height == rect.Height;

    /// <summary>
    /// Returns the hash code for this RectangleFloat.
    /// </summary>
    /// <returns>The hash code for this RectangleFloat as a 32-bit signed integer.</returns>
    public override readonly int GetHashCode() =>
        HashCode.Combine(Left, Top, Width, Height);

    /// <summary>
    /// Returns a value that indicates if the RectangleFloat on the left-hand side of the equality operator
    /// is equal to the RectangleFloat on the right-hand side of the equality operator.
    /// </summary>
    /// <param name="rectA">The RectangleFloat on the left-hand side of the equality operator.</param>
    /// <param name="rectB">The RectangleFloat on the right-hand side of the equality operator.</param>
    /// <returns>Whether the two RectangleFloats are equal.</returns>
    public static bool operator ==(RectangleFloat rectA, RectangleFloat rectB) =>
        rectA.Equals(rectB);

    /// <summary>
    /// Returns a value that indicates if the RectangleFloat on the left-hand side of the inequality operator
    /// is not equal to the RectangleFloat on the right-hand side of the inequality operator.
    /// </summary>
    /// <param name="rectA">The RectangleFloat on the left-hand side of the inequality operator.</param>
    /// <param name="rectB">The RectangleFloat on the right-hand side of the inequality operator.</param>
    /// <returns>Whether the two RectangleFloats are not equal.</returns>
    public static bool operator !=(RectangleFloat rectA, RectangleFloat rectB) =>
        !rectA.Equals(rectB);

    /// <summary>
    /// Clones the RectangleFloat instance (deep clone).
    /// </summary>
    /// <returns>Returns the cloned item.</returns>
    public RectangleFloat Clone()
    {
        return (RectangleFloat)MemberwiseClone();
    }
    #endregion Public Methods
}
