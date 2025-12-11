/***************************************************************
 * File: Camera.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Camera class provides translation and scaling functionality
 *  for 2D rendering in MonoGame. It defines a transformation matrix
 *  used with SpriteBatch to render the scene from a given position
 *  and zoom level. The camera operates in a top-left coordinate system
 *  and enforces a singleton pattern to ensure only one instance exists
 *  at runtime.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework;

#nullable enable

namespace CoreLibrary
{
    /// <summary>
    /// Represents the 2D camera used for rendering the scene.
    /// The camera controls translation (position) and scale (zoom)
    /// for all rendered objects.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The global reference to the active Camera instance.
        /// </summary>
        internal static Camera? s_instance;

        /// <summary>
        /// Gets a reference to the global Camera instance.
        /// </summary>
        public static Camera Instance => s_instance ?? throw new InvalidOperationException("Camera has not been initialized.");

        /// <summary>
        /// Gets or sets the XY-coordinate translation of the camera,
        /// relative to the top-left corner of the scene.
        /// </summary>
        public Vector2 Translation { get; set; }

        /// <summary>
        /// Gets or sets the scale (zoom level) of the camera.
        /// A value of 1.0 represents default zoom.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Gets the transformation matrix combining translation and scale.
        /// Used when rendering with <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch"/>.
        /// </summary>
        public Matrix TransformationMatrix
        {
            get
            {
                float tx = (float)Math.Floor(Translation.X);
                float ty = (float)Math.Floor(Translation.Y);

                // Scale is applied before translation (correct order for 2D).
                return Matrix.CreateScale(Scale) * Matrix.CreateTranslation(-tx, -ty, 0);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class
        /// at the origin with a default scale of 1.0.
        /// </summary>
        public Camera() : this(Vector2.Zero, 1f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="position">The starting translation of the camera.</param>
        /// <param name="scale">The starting scale (zoom) of the camera.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if more than one Camera instance is created.
        /// </exception>
        public Camera(Vector2 position, float scale = 1f)
        {
            if (s_instance != null)
                throw new InvalidOperationException("Only a single Camera instance can be created.");

            s_instance = this;
            Translation = position;
            Scale = scale;
        }

        /// <summary>
        /// Converts a screen-space position to world-space coordinates.
        /// </summary>
        public Vector2 GetWorldPosition(Vector2 screenPosition)
        {
            return screenPosition / Scale + Translation;
        }

        /// <summary>
        /// Converts a world-space position to screen-space coordinates.
        /// </summary>
        public Vector2 GetScreenPosition(Vector2 worldPosition)
        {
            return (worldPosition - Translation) * Scale;
        }

        /// <summary>
        /// Returns the world-space bounds currently visible through the camera.
        /// </summary>
        /// <param name="screenWidth">The width of the viewport in pixels.</param>
        /// <param name="screenHeight">The height of the viewport in pixels.</param>
        /// <returns>
        /// A <see cref="RectangleFloat"/> representing the visible area.
        /// </returns>
        public RectangleFloat GetBounds(float screenWidth, float screenHeight)
        {
            float width = screenWidth / Scale;
            float height = screenHeight / Scale;

            return new RectangleFloat(Translation.X, Translation.Y, width, height);
        }
    }
}
