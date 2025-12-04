/***************************************************************
 * File: TextureAtlas.cs
 * Author: FarLostBrand
 * Date: November 26, 2025
 * 
 * Summary:
 *  The TextureAtlas class manages a collection of texture regions
 *  and animations derived from a single source texture. It allows
 *  regions and animations to be programmatically added, retrieved,
 *  or loaded from XML configuration files for efficient sprite and
 *  animation management in 2D games.
 * 
 * License:
 *  © 2025 FarLostBrand. All rights reserved.
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CoreLibrary.Graphics;

/// <summary>
/// Represents a collection of named texture regions and animations
/// derived from a single source texture.
/// </summary>
public class TextureAtlas
{
    #region Fields

    private readonly Dictionary<string, TextureRegion> _regions;
    private readonly Dictionary<string, Animation> _animations;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets or sets the source texture represented by this texture atlas.
    /// </summary>
    public Texture2D Texture { get; set; }

    /// <summary>
    /// Gets the file name this TextureAtlas is derived from.
    /// </summary>
    public string FileName {get; private set;}

    #endregion Properties

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureAtlas"/> class.
    /// </summary>
    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureAtlas"/> class
    /// with the specified source texture.
    /// </summary>
    /// <param name="texture">The texture to associate with this atlas.</param>
    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    #endregion Constructors

    #region Region Management

    /// <summary>
    /// Adds a new region to this texture atlas.
    /// </summary>
    public void AddRegion(string name, int x, int y, int width, int height)
    {
        var region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    /// <summary>
    /// Retrieves the texture region with the specified name.
    /// </summary>
    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    /// <summary>
    /// Removes the texture region with the specified name.
    /// </summary>
    public bool RemoveRegion(string name)
    {
        return _regions.Remove(name);
    }

    /// <summary>
    /// Removes all texture regions from this atlas.
    /// </summary>
    public void Clear()
    {
        _regions.Clear();
    }

    #endregion Region Management

    #region Animation Management

    /// <summary>
    /// Adds an animation to this texture atlas.
    /// </summary>
    public void AddAnimation(string animationName, Animation animation)
    {
        _animations.Add(animationName, animation);
    }

    /// <summary>
    /// Retrieves an animation by its name.
    /// </summary>
    public Animation GetAnimation(string animationName)
    {
        return _animations[animationName];
    }

    /// <summary>
    /// Removes an animation from this texture atlas.
    /// </summary>
    public bool RemoveAnimation(string animationName)
    {
        return _animations.Remove(animationName);
    }

    #endregion Animation Management

    #region Factory Methods

    /// <summary>
    /// Creates a new sprite using a region from this texture atlas.
    /// </summary>
    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    /// <summary>
    /// Creates a new animated sprite using an animation from this texture atlas.
    /// </summary>
    /// <param name="animationName">The name of the animation.</param>
    /// <param name="totalCycles">The amount of times you want the animation to run. Default is the max possible.</param>
    /// <returns>Returns a new Animated Sprite.</returns>
    public AnimatedSprite CreateAnimatedSprite(string animationName, int totalCycles = int.MaxValue)
    {
        Animation animation = GetAnimation(animationName);
        return new AnimatedSprite(animation, totalCycles);
    }

    #endregion Factory Methods

    #region Static Methods

    /// <summary>
    /// Loads a texture atlas from an XML configuration file.
    /// </summary>
    /// <param name="content">The ContentManager used to load the texture.</param>
    /// <param name="fileName">The XML file path, relative to the content root directory.</param>
    /// <returns>A fully constructed <see cref="TextureAtlas"/> instance.</returns>
    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        var atlas = new TextureAtlas();
        string filePath = Path.Combine(content.RootDirectory, fileName);

        using Stream stream = TitleContainer.OpenStream(filePath);
        using XmlReader reader = XmlReader.Create(stream);
        XDocument doc = XDocument.Load(reader);
        XElement root = doc.Root ?? throw new InvalidOperationException("Texture atlas XML is missing a root element.");

        // Load texture
        string texturePath = root.Element("Texture")?.Value 
            ?? throw new InvalidOperationException("Texture element missing in atlas XML.");
        atlas.Texture = content.Load<Texture2D>(texturePath);

        // Load regions
        var regions = root.Element("Regions")?.Elements("Region");
        if (regions != null)
        {
            foreach (var region in regions)
            {
                string name = region.Attribute("name")?.Value;
                int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                if (!string.IsNullOrEmpty(name))
                    atlas.AddRegion(name, x, y, width, height);
            }
        }

        // Load animations
        var animationElements = root.Element("Animations")?.Elements("Animation");
        if (animationElements != null)
        {
            foreach (var animationElement in animationElements)
            {
                string name = animationElement.Attribute("name")?.Value;
                float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                var frames = new List<TextureRegion>();
                foreach (var frameElement in animationElement.Elements("Frame"))
                {
                    string regionName = frameElement.Attribute("region")?.Value;
                    if (regionName != null && atlas._regions.ContainsKey(regionName))
                    {
                        frames.Add(atlas._regions[regionName]);
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    var animation = new Animation(frames, delay);
                    atlas.AddAnimation(name, animation);
                }
            }
        }

        // Sets the filename in the atlas.
        atlas.FileName = fileName;

        return atlas;
    }

    #endregion Static Methods
}
