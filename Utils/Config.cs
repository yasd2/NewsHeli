namespace NewsHeli.Utils;

internal class Config
{
    public static bool EnableAIDispatch = true;

    public static int ArrivalTimeHeli = 30;

    public static int ArrivalTimeVan = 40;

    public static Boolean EnableHeli = true;

    public static Boolean EnableVan = true;

    public static void Read()
    {
        var ini = new InitializationFile("plugins/LSPDFR/MCCallouts.ini");

        if (!ini.Exists())
        {
            Game.LogTrivial("Could not locate 'plugins/LSPDFR/NewsHeli.ini' file!");
            return;
        }

        EnableAIDispatch = ini.ReadBoolean("Settings", "EnableAIDispatch", EnableAIDispatch);

        ArrivalTimeHeli = ini.ReadInt32("Settings", "ArrivalTimeHeli", ArrivalTimeHeli);

        ArrivalTimeVan = ini.ReadInt32("Settings", "ArrivalTimeVan", ArrivalTimeVan);

        EnableHeli = ini.ReadBoolean("Settings", "EnableHeli", EnableHeli);

        EnableVan = ini.ReadBoolean("Settings", "EnableVan", EnableVan);
    }
}
