namespace NewsHeli.Utils;

internal static class GameFiberExtentions
{
    internal static void SafeAbort(this GameFiber gameFiber)
    {
        if (gameFiber == null) return;

        if (!gameFiber.IsAlive) return;

        Logger.Log($"GF {gameFiber.Name} was safely aborted.");
        gameFiber.Abort();
    }
}
