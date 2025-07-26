namespace NewsHeli.Utils.Extentions;

/// <summary>
/// Helpful static extention commands for easier developing.
/// </summary>
internal static class MathExtentions
{
    internal static int RandomNumber(int min, int max)
        => MathHelper.GetRandomInteger(min, max);

    internal static int RandomNumber(int max)
        => MathHelper.GetRandomInteger(max);

    internal static int RandomHeading()
        => MathHelper.GetRandomInteger(361);

    public static T UseRandom<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("The list can't be null or empty.");

        return list[RandomNumber(list.Count)];
    }
}
