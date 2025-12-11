
using System;
using System.IO;
using System.Linq;
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
/// <param name="hasDash"> Whether the player has it's dash this level.</param>
/// <param name="hasPhase"> Whether the player has it's phase this level.</param>
/// <param name="targets">The targets in this level. Note list stores target's health, the length is the number of targets.</param>
/// <param name="enemies">The enemies in this level. Note list stores enemy's health, the length is the number of enemies.</param>
public record Level(LevelType type, Color color, string tilemapPath, bool hasDash, bool hasPhase, int[] targets, int[] enemies)
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

            // Player's dash.
            bool hasDash = bool.Parse(levelElement.Attribute("hasDash").Value);

            // Player's phase.
            bool hasPhase = bool.Parse(levelElement.Attribute("hasPhase").Value);

            // Targets in level.
            XElement targetsElement = levelElement.Element("Targets");
            // The LINQ from Web was so helpful :D.
            int[] targets = targetsElement != null
            // If is true.
            ? targetsElement.Elements("Target")
                .Select(t => int.Parse(t.Attribute("health").Value))
                .ToArray()
            // If is false.
            : Array.Empty<int>();

            // Enemies in level.
            XElement enemiesElement = levelElement.Element("Enemies");
            // More LINQ from Web to get the enemies.
            int[] enemies = enemiesElement != null
            // If is true.
            ? enemiesElement.Elements("Enemy")
                .Select(e => int.Parse(e.Attribute("health").Value))
                .ToArray()
            // If is false.
            : Array.Empty<int>();

            return new Level(levelType, color, tilemapPath, hasDash, hasPhase, targets, enemies);
        }
    }
}