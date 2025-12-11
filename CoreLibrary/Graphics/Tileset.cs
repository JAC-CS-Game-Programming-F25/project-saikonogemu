/***************************************************************
 * File: Tileset.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The Tileset class represents a collection of uniformly sized 
 *  tiles extracted from a larger texture region. It provides
 *  functionality for retrieving individual tiles based on 
 *  index, row, or column position.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;

namespace CoreLibrary.Graphics;

/// <summary>
/// Represents a collection of texture regions that form a tile set.
/// </summary>
public class Tileset
{
    #region Fields

    private readonly TextureRegion[] _tiles;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the width, in pixels, of each tile in this tileset.
    /// </summary>
    public int TileWidth { get; }

    /// <summary>
    /// Gets the height, in pixels, of each tile in this tileset.
    /// </summary>
    public int TileHeight { get; }

    /// <summary>
    /// Gets the total number of columns in this tileset.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the total number of rows in this tileset.
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// Gets the total number of tiles contained in this tileset.
    /// </summary>
    public int Count { get; }

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Creates a new tileset from the specified texture region and tile size.
    /// </summary>
    /// <param name="textureRegion">The texture region containing the tiles.</param>
    /// <param name="tileWidth">The width, in pixels, of each tile.</param>
    /// <param name="tileHeight">The height, in pixels, of each tile.</param>
    public Tileset(TextureRegion textureRegion, int tileWidth, int tileHeight)
    {
        TileWidth = tileWidth;
        TileHeight = tileHeight;

        Columns = textureRegion.Width / tileWidth;
        Rows = textureRegion.Height / tileHeight;
        Count = Columns * Rows;

        _tiles = new TextureRegion[Count];

        // Generate texture regions for each tile
        for (int i = 0; i < Count; i++)
        {
            int x = (i % Columns) * tileWidth;
            int y = (i / Columns) * tileHeight;

            _tiles[i] = new TextureRegion(
                textureRegion.Texture,
                textureRegion.SourceRectangle.X + x,
                textureRegion.SourceRectangle.Y + y,
                tileWidth,
                tileHeight
            );
        }
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Gets the texture region for the tile at the specified index.
    /// </summary>
    /// <param name="index">The index of the tile to retrieve.</param>
    /// <returns>The texture region representing the tile at the given index.</returns>
    public TextureRegion GetTile(int index) => _tiles[index]; 

    /// <summary>
    /// Gets the texture region for the tile at the specified column and row.
    /// </summary>
    /// <param name="column">The column index of the tile within the tileset.</param>
    /// <param name="row">The row index of the tile within the tileset.</param>
    /// <returns>The texture region representing the tile at the given location.</returns>
    public TextureRegion GetTile(int column, int row)
    {
        int index = row * Columns + column;
        return GetTile(index);
    }

    #endregion Public Methods
}
