/***************************************************************
 * File: Tilemap.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Tilemap class represents a 2D grid of tiles rendered from a 
 *  given tileset. It handles tile rendering, collision data, and 
 *  can be created from an XML configuration file defining layout 
 *  and collision attributes.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using CoreLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CoreLibrary.Graphics;

/// <summary>
/// Represents a 2D grid of tiles that can be rendered and 
/// used for collision detection. Supports loading from an XML 
/// configuration file and integrates with <see cref="Tileset"/>.
/// </summary>
public class Tilemap
{
    #region Fields

    private readonly Tileset _tileset;
    private readonly int?[] _tiles;
    private readonly bool[] _collidable;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the total number of rows in this tilemap.
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// Gets the total number of columns in this tilemap.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the total number of tiles in this tilemap.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets or sets the scale factor to draw each tile at.
    /// </summary>
    public Vector2 Scale { get; set; }

    /// <summary>
    /// Gets the width, in pixels, each tile is drawn at.
    /// </summary>
    public float TileWidth => _tileset.TileWidth * Scale.X;

    /// <summary>
    /// Gets the height, in pixels, each tile is drawn at.
    /// </summary>
    public float TileHeight => _tileset.TileHeight * Scale.Y;

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="Tilemap"/> using the specified tileset, column, and row counts.
    /// </summary>
    /// <param name="tileset">The tileset used by this tilemap.</param>
    /// <param name="columns">The total number of columns in this tilemap.</param>
    /// <param name="rows">The total number of rows in this tilemap.</param>
    public Tilemap(Tileset tileset, int columns, int rows)
    {
        _tileset = tileset;
        Rows = rows;
        Columns = columns;
        Count = Columns * Rows;
        Scale = Vector2.One;
        _tiles = new int?[Count];
        _collidable = new bool[Count];
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Sets the tile at the given index in this tilemap to use the tile from
    /// the tileset at the specified tileset id.
    /// </summary>
    /// <param name="index">The index of the tile in this tilemap.</param>
    /// <param name="tilesetID">The tileset id of the tile from the tileset to use.</param>
    /// <param name="collidable">Whether the tile is collidable, default is false.</param>
    public void SetTile(int index, int? tilesetID, bool collidable = false)
    {
        _tiles[index] = tilesetID;
        _collidable[index] = collidable && tilesetID.HasValue;
    }

    /// <summary>
    /// Sets the tile at the given column and row in this tilemap to use the tile
    /// from the tileset at the specified tileset id.
    /// </summary>
    /// <param name="column">The column of the tile in this tilemap.</param>
    /// <param name="row">The row of the tile in this tilemap.</param>
    /// <param name="tilesetID">The tileset id of the tile from the tileset to use.</param>
    public void SetTile(int column, int row, int tilesetID, bool collidable = false)
    {
        int index = row * Columns + column;
        SetTile(index, tilesetID, collidable);
    }

    /// <summary>
    /// Gets the texture region of the tile from this tilemap at the specified index.
    /// </summary>
    /// <param name="index">The index of the tile in this tilemap.</param>
    /// <returns>The texture region of the tile from this tilemap at the specified index.</returns>
    public TextureRegion GetTile(int index)
    {
        int? id = _tiles[index];
        return id is null or < 0 ? null : _tileset.GetTile(id.Value) ;
    }

    /// <summary>
    /// Gets the texture region of the tile from this tilemap at the specified
    /// column and row.
    /// </summary>
    /// <param name="column">The column of the tile in this tilemap.</param>
    /// <param name="row">The row of the tile in this tilemap.</param>
    /// <returns>The texture region of the tile from this tilemap at the specified column and row.</returns>
    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    /// <summary>
    /// Gets whether the tile at the given index is collidable.
    /// </summary>
    /// <param name="index">The index of the tile in this tilemap.</param>
    /// <returns><see langword="true"/> if the tile is collidable; otherwise, <see langword="false"/>.</returns>
    public bool IsCollidable(int index)
    {
        return _collidable[index];
    }

    /// <summary>
    /// Gets whether the tile at the given column and row is collidable.
    /// </summary>
    /// <param name="column">The column of the tile in this tilemap.</param>
    /// <param name="row">The row of the tile in this tilemap.</param>
    /// <returns><see langword="true"/> if the tile is collidable; otherwise, <see langword="false"/>.</returns>
    public bool IsCollidable(int column, int row)
    {
        int index = row * Columns + column;
        return IsCollidable(index);
    }

    /// <summary>
    /// Draws this tilemap using the given sprite batch.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used to draw this tilemap.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < Count; i++)
        {
            int? tileSetIndex = _tiles[i];

            if (tileSetIndex == null || tileSetIndex < 0)
                continue;

            TextureRegion tile = _tileset.GetTile(tileSetIndex.Value);

            int x = i % Columns;
            int y = i / Columns;

            Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
            tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
        }
    }

    #region Collision Helpers

    /// <summary>
    /// Gets the collider rectangle for a provided tile.
    /// </summary>
    /// <param name="column">The column of the specified tile.</param>
    /// <param name="row">The row of the specified tile.</param>
    /// <returns>The rectangle collision bounds of the tile.</returns>
    public RectangleFloat GetTileBounds(int column, int row)
    {
        return new RectangleFloat(
            column * TileWidth,
            row * TileHeight,
            TileWidth,
            TileHeight);
    }

    /// <summary>
    /// Checks all collidable tiles that are near or overlapping the given rectangle.
    /// </summary>
    /// <param name="area">The area to check for overlapping collidable tiles.</param>
    /// <returns>All collidable tile rectangles overlapping or near the given area.</returns>
    public IEnumerable<RectangleFloat> GetNearbyColliders(RectangleFloat area)
    {
        int leftTile = Math.Max(0, (int)Math.Floor(area.Left / TileWidth));
        int rightTile = Math.Min(Columns - 1, (int)Math.Floor(area.Right / TileWidth));
        int topTile = Math.Max(0, (int)Math.Floor(area.Top / TileHeight));
        int bottomTile = Math.Min(Rows - 1, (int)Math.Floor(area.Bottom / TileHeight));

        for (int y = topTile; y <= bottomTile; y++)
        {
            for (int x = leftTile; x <= rightTile; x++)
            {
                if (IsCollidable(x, y))
                    yield return GetTileBounds(x, y);
            }
        }
    }

    /// <summary>
    /// Draws rectangles around the colliders.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for drawing.</param>
    /// <param name="pixel">The texture used to draw the rectangles.</param>
    /// <param name="color">The color of the rectangles.</param>
    public void DrawColliders(SpriteBatch spriteBatch, Texture2D pixel, Color color)
    {
        for (int i = 0; i < Count; i++)
        {
            if (_collidable[i])
            {
                int x = i % Columns;
                int y = i / Columns;
                RectangleFloat rect = new RectangleFloat(x * TileWidth, y * TileHeight, TileWidth, TileHeight);
                spriteBatch.Draw(pixel, rect.Position, color * 0.4f);
            }
        }
    }

    #endregion Collision Helpers

    /// <summary>
    /// Creates a new tilemap based on a tilemap XML configuration file.
    /// </summary>
    /// <param name="content">The content manager used to load the texture for the tileset.</param>
    /// <param name="filename">The path to the XML file, relative to the content root directory.</param>
    /// <returns>The tilemap created by this method.</returns>
    public static Tilemap FromFile(ContentManager content, string filename)
    {
        string filePath = Path.Combine(content.RootDirectory, filename);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        using (XmlReader reader = XmlReader.Create(stream))
        {
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            // The <Tileset> element contains the information about the tileset
            // used by the tilemap.
            //
            // Example
            // <Tileset region="0 0 100 100" tileWidth="10" tileHeight="10">contentPath</Tileset>
            //
            // The region attribute represents the x, y, width, and height
            // components of the boundary for the texture region within the
            // texture at the contentPath specified.
            //
            // The tileWidth and tileHeight attributes specify the width and
            // height of each tile in the tileset.
            //
            // The contentPath value is the path to the texture to
            // load that contains the tileset.
            XElement tilesetElement = root.Element("Tileset");

            string regionAttribute = tilesetElement.Attribute("region").Value;
            string[] split = regionAttribute.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);
            int width = int.Parse(split[2]);
            int height = int.Parse(split[3]);

            int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
            int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
            string contentPath = tilesetElement.Value;

            // Load the texture 2D at the content path
            Texture2D texture = content.Load<Texture2D>(contentPath);

            // Create the texture region from the texture
            TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

            // Create the tileset using the texture region
            Tileset tileset = new Tileset(textureRegion, tileWidth, tileHeight);

            // The <Tiles> element contains lines of strings where each line
            // represents a row in the tilemap. Each line is a space-separated
            // string where each element represents a column in that row.
            //
            // Example:
            // <Tiles>
            //      00 01 01 02
            //      03 04 04 05
            //      03 04 04 05
            //      06 07 07 08
            // </Tiles>
            XElement tilesElement = root.Element("Tiles");

            // Get the collidable tiles from the tilemap definition.
            HashSet<int> collidableTiles = new HashSet<int>();
            XElement collidableElement = root.Element("CollidableTiles");
            if (collidableElement != null)
            {
                string[] solidIds = collidableElement.Value
                    .Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (var id in solidIds)
                    collidableTiles.Add(int.Parse(id));
            }

            // Split the value of the tiles data into rows by splitting on
            // the new line character.
            string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // Split the value of the first row to determine the total number of columns.
            int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

            // Create the tilemap
            Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

            // Process each row
            for (int row = 0; row < rows.Length; row++)
            {
                // Split the row into individual columns
                string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                // Process each column of the current row
                for (int column = 0; column < columnCount; column++)
                {
                    // Get the tileset index for this location
                    int tilesetIndex = int.Parse(columns[column]) - 1;

                    if (tilesetIndex < 0)
                        continue;

                    // Get the texture region of that tile from the tileset
                    TextureRegion region = tileset.GetTile(tilesetIndex);

                    // Get whether that tile should be collidable
                    bool collidable = collidableTiles.Contains(tilesetIndex);

                    // Add that region to the tilemap at the row and column location
                    tilemap.SetTile(column, row, tilesetIndex, collidable);
                }
            }

            return tilemap;
        }
    }

    #endregion Public Methods
}
