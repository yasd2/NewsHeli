namespace NewsHeli.Utils;

/// <summary>
/// A logger class for development. Logs data in the RPH.log.
/// </summary>
internal class Logger
{
    public static void Log(string text)
        => Game.LogTrivial("NewsHeli: " + text);
}
