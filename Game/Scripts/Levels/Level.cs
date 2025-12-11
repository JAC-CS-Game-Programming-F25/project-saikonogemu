
using System.IO;
using System.Xml;
using System.Xml.Linq;
using CoreLibrary.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Game.Scripts.Levels;

/// <summary>
/// A data class responsible to hold all the level information of a level.
/// </summary>
/// <param name="type">The level type (or id). E.g. Level1.</param>
/// <param name="color">The color associated with the level, used when rendering backgrounds.</param>
/// <param name="tilemapPath">The path to the tilemap info.</param>
/// <param name="numberOfTargets">The amount of targets in this level.</param>
/// <param name="numberOfEnemies">The amount of enemies in this level.</param>
public record Level(LevelType type, Color color, string tilemapPath, int numberOfTargets, int numberOfEnemies)
{
    /// <summary>
    /// Creates a Level based on a xml file.
    /// </summary>
    /// <param name="content">The content manager used to load the information for the level.</param>
    /// <param name="levelDataFileName">The file name of the xml containing the level's information.</param>
    /// <returns>Returns the newly created level from the xml.</returns>
    public static Level FromFile(ContentManager content, string levelDataFileName)
    {
        string filePath = Path.Combine(content.RootDirectory, levelDataFileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        using (XmlReader reader = XmlReader.Create(stream))
        {
            XDocument doc = XDocument.Load(reader);
            XElement root = doc.Root;

            // The <Level> element contains the information about the level.
            XElement levelElement = doc.Root;

            // All the level information.
            // Level type.
            LevelType levelType = (LevelType)int.Parse(levelElement.Attribute("levelNumber").Value);

            // Level color.
            Color color = Utils.FromHex(levelElement.Attribute("color").Value);

            // Level's tilemap path.
            string tilemapPath = levelElement.Attribute("tilemapPath").Value;

            // The number of targets in this Level.
            int numberOfTargets = int.Parse(levelElement.Attribute("numberOfTargets").Value);

            // The number of enemies in this Level.
            int numberOfEnemies = int.Parse(levelElement.Attribute("numberOfEnemies").Value);

            return new Level(levelType, color, tilemapPath, numberOfTargets, numberOfEnemies);
        }
    }
}