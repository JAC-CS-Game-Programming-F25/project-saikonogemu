using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Game.Scripts.Entities.Dice;

public static class DiceFactory
{
    /// <summary>
    /// Creates a dice based on it's type.
    /// </summary>
    /// <param name="diceType">The type of dice you would like to make.</param>
    /// <param name="content">The content used to load content into the game.</param>
    /// <param name="diceOptions">The dice parameters needed to create a dice.</param>
    /// <returns>Returns a new Dice of the correct type.</returns>
    public static Dice CreateDice(DiceTypes diceType, ContentManager content, Dictionary<string, object> diceOptions)
    {
        // VS code swapped my switch case statement to this and this is insanely cool.
        return diceType switch
        {
            DiceTypes.Player => new PlayerDice(content, diceOptions),
            DiceTypes.Target => new TargetDice(content, diceOptions),
            DiceTypes.Enemy => new EnemyDice(content, diceOptions),
            _ => new Dice(content, diceOptions),
        };
    }
}