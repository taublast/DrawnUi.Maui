namespace SpaceShooter.Game;

public enum GameState
{
    Unset,

    /// <summary>
    /// Welcome screen presented
    /// </summary>
    Ready,

    /// <summary>
    /// Game loop is running
    /// </summary>
    Playing,

    /// <summary>
    /// TODO
    /// </summary>
    Paused,

    /// <summary>
    /// Game ended
    /// </summary>
    Ended
}