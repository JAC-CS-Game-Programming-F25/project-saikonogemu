using System.Collections.Generic;

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
    #endregion Helper Methods
}