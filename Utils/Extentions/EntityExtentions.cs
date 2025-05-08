namespace NewsHeli.Utils;

internal static class EntityExtentions
{
    internal static void DeleteIfExists(this Entity entity)
    {
        if (entity) entity.Delete();
    }

    internal static void DeleteIfExists(this Blip blip)
    {
        if (blip) blip.Delete();
    }

    internal static void DismissIfExists(this Entity entity)
    {
        if (entity) entity.Dismiss();
    }
}
