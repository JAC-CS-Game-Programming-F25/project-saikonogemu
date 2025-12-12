/// <summary>
/// The graphics settings of the game.
/// </summary>
public class GraphicsSettings
{
    /// <summary>
    /// The resolution width of the game.
    /// Default is 512.
    /// </summary>
    public int Width { get; set; } = 1920;

    /// <summary>
    /// The resolution height of the game.
    /// Default is 256.
    /// </summary>
    public int Height { get; set; } = 1080;

    /// <summary>
    /// Whether the game is fullscreen.
    /// Default is false.
    /// </summary>
    public bool Fullscreen { get; set; } = true;
}