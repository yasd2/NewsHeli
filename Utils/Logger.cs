namespace NewsHeli.Utils;

internal class Logger
{
    public static void Log(string text)
        => Game.LogTrivial("NewsHeli: " + text);
}
